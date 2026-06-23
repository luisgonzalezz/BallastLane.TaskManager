using System.Text.RegularExpressions;
using BallastLane.TaskManager.Domain.Exceptions;

namespace BallastLane.TaskManager.Domain.Entities;

public sealed class User
{
    private static readonly Regex EmailPattern = new(
        "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private User(Guid id, string email, string passwordHash, DateTime createdAt)
    {
        Id = id;
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }

    public string Email { get; }

    public string PasswordHash { get; }

    public DateTime CreatedAt { get; }

    public static User Create(string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(email) || !EmailPattern.IsMatch(email))
        {
            throw new DomainValidationException("A valid email is required.");
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainValidationException("Password hash is required.");
        }

        return new User(Guid.NewGuid(), email.Trim(), passwordHash, DateTime.UtcNow);
    }
}
