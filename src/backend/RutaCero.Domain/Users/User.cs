using RutaCero.Domain.Common;

namespace RutaCero.Domain.Users;

public sealed class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    public User(string email, string passwordHash, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new DomainException("Email is invalid.");
        Id = Guid.NewGuid();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        CreatedAt = createdAt.ToUniversalTime();
    }

    private User() { Email = PasswordHash = string.Empty; }
}
