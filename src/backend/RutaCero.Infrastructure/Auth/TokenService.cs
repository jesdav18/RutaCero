using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RutaCero.Application.Auth;
using RutaCero.Domain.Users;

namespace RutaCero.Infrastructure.Auth;

public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(User user)
    {
        var expires = DateTimeOffset.UtcNow.AddMinutes(15);
        var key = configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is required.");
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Email, user.Email) };
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), Expires = expires.UtcDateTime,
            Issuer = configuration["Jwt:Issuer"], Audience = configuration["Jwt:Audience"],
            SigningCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256)
        };
        var handler = new JwtSecurityTokenHandler();
        return (handler.WriteToken(handler.CreateToken(descriptor)), expires);
    }

    public string CreateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    public string HashRefreshToken(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
