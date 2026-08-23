using Microsoft.EntityFrameworkCore;
using RutaCero.Application.Auth;
using RutaCero.Domain.Users;

namespace RutaCero.Infrastructure.Persistence;

public sealed class UserRepository(RutaCeroDbContext db) : IUserRepository
{
    public Task<User?> FindByEmailAsync(string email, CancellationToken token) =>
        db.Users.SingleOrDefaultAsync(x => x.Email == email, token);
    public Task<User?> FindByIdAsync(Guid id, CancellationToken token) => db.Users.FindAsync([id], token).AsTask();
    public async Task AddAsync(User user, CancellationToken token) => await db.Users.AddAsync(user, token);
}

public sealed class RefreshTokenRepository(RutaCeroDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> FindByHashAsync(string hash, CancellationToken token) =>
        db.RefreshTokens.SingleOrDefaultAsync(x => x.TokenHash == hash, token);
    public async Task AddAsync(RefreshToken refreshToken, CancellationToken token) =>
        await db.RefreshTokens.AddAsync(refreshToken, token);
}
