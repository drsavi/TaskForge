using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Infrastructure.Services
{
    public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
    {
        public string UserId =>
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");

        public bool IsAuthenticated =>
            httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;
    }
}
