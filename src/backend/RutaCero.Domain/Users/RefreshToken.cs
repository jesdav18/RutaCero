namespace RutaCero.Domain.Users;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string TokenHash { get; private set; }
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? RevokedAt { get; private set; }
    public Guid? ReplacedById { get; private set; }
    public bool IsActive(DateTimeOffset now) => RevokedAt is null && ExpiresAt > now;

    public RefreshToken(Guid userId, string hash, DateTimeOffset expiresAt, DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid(); UserId = userId; TokenHash = hash;
        ExpiresAt = expiresAt.ToUniversalTime(); CreatedAt = createdAt.ToUniversalTime();
    }

    public void Revoke(DateTimeOffset now, Guid? replacementId = null)
    {
        RevokedAt = now.ToUniversalTime(); ReplacedById = replacementId;
    }

    private RefreshToken() { TokenHash = string.Empty; }
}
