using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;
using TaskForge.Infrastructure.Identity;

namespace TaskForge.Api.Controllers
{
    [ApiController]
    [Route("api/sessions")]
    public class SessionsController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwtTokenService) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(SessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
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

        private UnauthorizedObjectResult InvalidCredentials() =>
            Unauthorized(new ProblemDetails
            {
                Title = "Invalid email or password.",
                Status = StatusCodes.Status401Unauthorized
            });
    }
}
