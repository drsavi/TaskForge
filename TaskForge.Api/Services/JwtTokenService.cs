using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using TaskForge.Application.Dtos;
using TaskForge.Application.Interfaces.Services;

namespace TaskForge.Api.Services
{
    public class JwtTokenService(IConfiguration config) : IJwtTokenService
    {
        public SessionDto CreateSession(string userId, string email)
        {
            var expireMinutes = config.GetValue("Jwt:ExpireMinutes", 60);
            var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]!);
            var issuer = config["Jwt:Issuer"]!;
            var audience = config["Jwt:Audience"]!;

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Email, email)
            };

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new SessionDto(accessToken, "Bearer", expireMinutes * 60);
        }
    }
}
