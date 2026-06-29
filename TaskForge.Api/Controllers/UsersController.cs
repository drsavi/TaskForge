using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Api.Infrastructure;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Infrastructure.Identity;

namespace TaskForge.Api.Controllers
{
    [Route("api/users")]
    public class UsersController(
        UserManager<ApplicationUser> userManager,
        ICurrentUserService currentUserService) : ApiControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<UserDto>> Register(RegisterUserRequest request)
        {
            var user = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                FullName = request.FullName
            };

            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = new Dictionary<string, string[]>
                {
                    ["identity"] = result.Errors.Select(e => e.Description).ToArray()
                };

                return ProblemDetailsResponseWriter.ProblemResult(
                    StatusCodes.Status400BadRequest,
                    new ValidationProblemDetails(errors)
                    {
                        Title = "Registration failed",
                        Status = StatusCodes.Status400BadRequest
                    });
            }

            var dto = ToDto(user);
            return CreatedAtAction(nameof(GetMe), dto);
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDto>> GetMe(CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(currentUserService.UserId);
            if (user is null)
                throw new KeyNotFoundException("User not found.");

            return Ok(ToDto(user));
        }

        private static UserDto ToDto(ApplicationUser user) =>
            new(user.Id, user.Email!, user.FullName);
    }
}
