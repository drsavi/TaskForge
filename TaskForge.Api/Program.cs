using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskForge.Application.Common.Behaviors;
using TaskForge.Application.Common.Validators;
using TaskForge.Application.Interfaces.Repositories;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Application.Projects.Commands.CreateProject;
using TaskForge.Infrastructure.Data;
using TaskForge.Infrastructure.Identity;
using TaskForge.Infrastructure.Repositories;
using TaskForge.Infrastructure.Services;

const string JwtKeyMissingMessage =
    "Jwt:Key is not configured. Options:\n" +
    "  Docker: set JWT_KEY in the .env file (see .env.example)\n" +
    "  Local:  dotnet user-secrets set \"Jwt:Key\" \"<your-dev-secret-at-least-32-chars>\" --project TaskForge.Api\n" +
    "  Or set the environment variable Jwt__Key.";

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException(
        "ConnectionStrings:Default is not configured. " +
        "Use appsettings.json defaults or set ConnectionStrings__Default.");

builder.Services.AddDbContext<TaskForgeDbContext>(opts =>
    opts.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorCodesToAdd: null
            );
        }
    ));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<TaskForgeDbContext>()
.AddDefaultTokenProviders();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException(JwtKeyMissingMessage);

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("Jwt:Issuer is missing in configuration.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("Jwt:Audience is missing in configuration.");

var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();

builder.Services
  .AddControllers()
  .AddJsonOptions(opts =>
  {
      opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
      opts.JsonSerializerOptions.WriteIndented = true;
      opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
  })
  .ConfigureApiBehaviorOptions(opts =>
  {
      opts.InvalidModelStateResponseFactory = context =>
      {
          var errors = context.ModelState
             .Where(x => x.Value?.Errors.Count > 0)
             .ToDictionary(
                 kvp => kvp.Key,
                 kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
             );

          var details = new ValidationProblemDetails(errors)
          {
              Title = "Validation failed",
              Status = StatusCodes.Status400BadRequest
          };

          return new BadRequestObjectResult(details);
      };
  });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskForge API",
        Version = "v1",
        Description = "API de gerenciamento de projetos. Endpoints de negócio exigem JWT; " +
                      "endpoints /health/* são públicos e destinados a monitoramento."
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Insira o token JWT assim: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id   = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddValidatorsFromAssemblyContaining<CreateProjectCommandValidator>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateProjectCommand).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.WriteIndented = true;
    options.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHealthChecks()
    .AddNpgSql(
        connectionString: connectionString,
        name: "postgresql",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "db", "ready" },
        timeout: TimeSpan.FromSeconds(3)
    );

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("ApplyMigrationsOnStartup"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TaskForgeDbContext>();
    db.Database.Migrate();
}

var enableSwagger = app.Environment.IsDevelopment()
    || builder.Configuration.GetValue<bool>("EnableSwagger");

if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (builder.Configuration.GetValue("UseHttpsRedirection", true))
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler(errApp =>
{
    errApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var ex = feature?.Error;

        if (ex is ValidationException vex)
        {
            var errors = vex.Errors
               .GroupBy(e => e.PropertyName)
               .ToDictionary(
                   g => g.Key,
                   g => g.Select(e => e.ErrorMessage).ToArray()
               );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationProblemDetails(errors));
            return;
        }

        if (ex is KeyNotFoundException)
        {
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = ex?.Message,
                Status = StatusCodes.Status404NotFound
            });
            return;
        }

        if (ex is UnauthorizedAccessException)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Title = ex?.Message,
                Status = StatusCodes.Status403Forbidden
            });
            return;
        }

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Title = "An unexpected error occurred.",
            Status = StatusCodes.Status500InternalServerError,
            Detail = ex?.Message
        });
    });
});

app.MapControllers();

app.Run();
