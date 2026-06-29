using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Api.Infrastructure;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Infrastructure.Identity;

namespace TaskForge.Api.Controllers
{
    [Route("api/sessions")]
    public class SessionsController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService) : ApiControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<SessionDto>> Create(CreateSessionRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user is null)
                return InvalidCredentials();

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!signInResult.Succeeded)
                return InvalidCredentials();

            var session = jwtTokenService.CreateSession(user.Id, user.Email!);
            return Created(string.Empty, session);
        }

        private ObjectResult InvalidCredentials() =>
            ProblemDetailsResponseWriter.ProblemResult(
                StatusCodes.Status401Unauthorized,
                new ProblemDetails
                {
                    Title = "Invalid email or password.",
                    Status = StatusCodes.Status401Unauthorized
                });
    }
}
