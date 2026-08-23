using RutaCero.Application.Common;
using RutaCero.Domain.Users;

namespace RutaCero.Application.Auth;

public sealed class AuthService(IUserRepository users, IRefreshTokenRepository refreshTokens,
    IPasswordService passwords, ITokenService tokens, IUnitOfWork unitOfWork)
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterCommand command, CancellationToken token)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        if (await users.FindByEmailAsync(email, token) is not null)
            return Result<AuthResponse>.Failure("El correo ya está registrado.");
        var user = new User(email, passwords.Hash(command.Password), DateTimeOffset.UtcNow);
        await users.AddAsync(user, token);
        var issued = await IssueAsync(user, token);
        await unitOfWork.SaveChangesAsync(token);
        return Result<AuthResponse>.Success(issued.Response);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginCommand command, CancellationToken token)
    {
        var user = await users.FindByEmailAsync(command.Email.Trim().ToLowerInvariant(), token);
        if (user is null || !passwords.Verify(user.PasswordHash, command.Password))
            return Result<AuthResponse>.Failure("Credenciales inválidas.");
        var issued = await IssueAsync(user, token);
        await unitOfWork.SaveChangesAsync(token);
        return Result<AuthResponse>.Success(issued.Response);
    }

    public async Task<Result<AuthResponse>> RefreshAsync(RefreshCommand command, CancellationToken token)
    {
        var current = await refreshTokens.FindByHashAsync(tokens.HashRefreshToken(command.RefreshToken), token);
        if (current is null || !current.IsActive(DateTimeOffset.UtcNow))
            return Result<AuthResponse>.Failure("La sesión ya no es válida.");
        var user = await users.FindByIdAsync(current.UserId, token);
        if (user is null) return Result<AuthResponse>.Failure("La sesión ya no es válida.");
        var issued = await IssueAsync(user, token);
        current.Revoke(DateTimeOffset.UtcNow,issued.Entity.Id);
        await unitOfWork.SaveChangesAsync(token);
        return Result<AuthResponse>.Success(issued.Response);
    }

    public async Task<bool> RevokeAsync(RefreshCommand command,CancellationToken token)
    {
        var current=await refreshTokens.FindByHashAsync(tokens.HashRefreshToken(command.RefreshToken),token);
        if(current is null||!current.IsActive(DateTimeOffset.UtcNow))return false;
        current.Revoke(DateTimeOffset.UtcNow);await unitOfWork.SaveChangesAsync(token);return true;
    }

    private async Task<(AuthResponse Response,RefreshToken Entity)> IssueAsync(User user, CancellationToken token)
    {
        var access = tokens.CreateAccessToken(user);
        var raw = tokens.CreateRefreshToken();
        var refresh = new RefreshToken(user.Id, tokens.HashRefreshToken(raw), DateTimeOffset.UtcNow.AddDays(30), DateTimeOffset.UtcNow);
        await refreshTokens.AddAsync(refresh, token);
        return (new(access.Token, raw, access.ExpiresAt),refresh);
    }
}
