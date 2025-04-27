using Microsoft.EntityFrameworkCore;
using TaskForge.Application.Services.Interfaces;
using TaskForge.Application.Services.Implementations;
using TaskForge.Infrastructure.Repositories.Implementations;
using TaskForge.Infrastructure.Repositories.Interfaces;
using TaskForge.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// 1. Configurar EF Core + PostgreSQL
builder.Services.AddDbContext<TaskForgeDbContext>(opts =>
    opts.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// 2. Registrar o padrão Repository
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// 3. Registrar o padrão Service
builder.Services.AddScoped<IProjectService, ProjectService>();

// 4. Controllers, Swagger e tudo mais
//builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();