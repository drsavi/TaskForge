using TaskForge.Application.Dtos;

namespace TaskForge.Application.Interfaces.Services
{
    public interface IJwtTokenService
    {
        SessionDto CreateSession(string userId, string email);
    }
}
