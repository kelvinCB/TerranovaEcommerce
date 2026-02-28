using Domain.Validations;
using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Represents a user verification entity in the system
/// </summary>
public class UserVerification
{
    public Ulid Id { get; private set; }
    public Ulid UserId { get; private set; }
    public string Purpose { get; private set; } = null!;
    public Code VerificationCode { get; private set; } = null!;
    public DateTimeOffset ExpiresAt { get; private set; }
    public DateTimeOffset? ConsumedAt { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserVerification"/> class with the specified parameters
    /// </summary>
    /// <param name="id">The verification identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="purpose">The purpose of the verification</param>
    /// <param name="verificationCode">The verification code</param>
    /// <param name="expiresAt">The expiration timestamp</param>
    /// <param name="createdAt">The timestamp when the verification was created</param>
    /// <remarks>
    /// Purpose e.g.: "email_verify", "phone_verify", "password_reset", "email_change"
    /// </remarks>
    private UserVerification(
        Ulid id,
        Ulid userId,
        string purpose,
        Code verificationCode,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt
    )
    {
        Id = id;
        UserId = userId;
        Purpose = purpose;
        VerificationCode = verificationCode;
        ExpiresAt = expiresAt;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="UserVerification"/> class with the specified parameters and performs necessary validations
    /// </summary>
    /// <param name="id">The verification identifier</param>
    /// <param name="userId">The user identifier</param>
    /// <param name="purpose">The purpose of the verification</param>
    /// <param name="verificationCode">The verification code</param>
    /// <param name="expiresAt">The expiration timestamp</param>
    /// <param name="createdAt">The timestamp when the verification was created</param>
    /// <returns>Returns the newly created <see cref="UserVerification"/> instance</returns>
    public static UserVerification Create(
        Ulid id,
        Ulid userId,
        string purpose,
        Code verificationCode,
        DateTimeOffset expiresAt,
        DateTimeOffset createdAt
    )
    {
        // Perform validations using the Guard class to ensure domain invariants are maintained
        Guard.EnsureUlidNotEmpty(id, nameof(id));
        Guard.EnsureUlidNotEmpty(userId, nameof(userId));
        Guard.EnsureStringNotNullOrWhiteSpace(purpose, nameof(purpose));
        Guard.EnsureNotNull(verificationCode, nameof(verificationCode));
        Guard.EnsureUtc(expiresAt, nameof(expiresAt));
        Guard.EnsureUtc(createdAt, nameof(createdAt));
        Guard.EnsureUtcNotBefore(expiresAt, createdAt, nameof(expiresAt));

        return new UserVerification(
            id,
            userId,
            purpose,
            verificationCode,
            expiresAt,
            createdAt
        );
    }

    /// <summary>
    /// Consumes the verification code and sets the consumed timestamp
    /// </summary>
    /// <param name="timestamp">The timestamp when the verification code was consumed</param>
    public void Consume(DateTimeOffset timestamp)
    {
        if (ConsumedAt.HasValue)
            throw new InvalidOperationException("The verification code has already been consumed");

        // Perform validations using the Guard class to ensure domain invariants are maintained
        Guard.EnsureUtc(timestamp, nameof(timestamp));
        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        if (timestamp > ExpiresAt)
            throw new InvalidOperationException("The verification code has expired");

        ConsumedAt = timestamp;
    }
}