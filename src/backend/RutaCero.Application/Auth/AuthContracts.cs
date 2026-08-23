using RutaCero.Domain.Users;

namespace RutaCero.Application.Auth;

public sealed record RegisterCommand(string Email, string Password);
public sealed record LoginCommand(string Email, string Password);
public sealed record RefreshCommand(string RefreshToken);
public sealed record AuthResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken token);
    Task<User?> FindByIdAsync(Guid id, CancellationToken token);
    Task AddAsync(User user, CancellationToken token);
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByHashAsync(string hash, CancellationToken token);
    Task AddAsync(RefreshToken refreshToken, CancellationToken token);
}

public interface IPasswordService
{
    string Hash(string password);
    bool Verify(string hash, string password);
}

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiresAt) CreateAccessToken(User user);
    string CreateRefreshToken();
    string HashRefreshToken(string token);
}
