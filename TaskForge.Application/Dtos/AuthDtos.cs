namespace TaskForge.Application.Dtos
{
    public record RegisterUserRequest(string FullName, string Email, string Password);

    public record CreateSessionRequest(string Email, string Password);

    public record UserDto(string Id, string Email, string? FullName);

    public record SessionDto(string AccessToken, string TokenType, int ExpiresIn);
}
