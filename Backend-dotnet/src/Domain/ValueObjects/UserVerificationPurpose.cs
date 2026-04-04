using Domain.Validations;

namespace Domain.ValueObjects;

/// <summary>
/// Represents the purpose of a user verification flow.
/// </summary>
public sealed record UserVerificationPurpose
{
    /// <summary>
    /// Gets the raw purpose value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Email verification purpose.
    /// </summary>
    public static UserVerificationPurpose EmailVerify { get; } = new("email_verify");

    /// <summary>
    /// Phone verification purpose.
    /// </summary>
    public static UserVerificationPurpose PhoneVerify { get; } = new("phone_verify");

    /// <summary>
    /// Password reset purpose.
    /// </summary>
    public static UserVerificationPurpose PasswordReset { get; } = new("password_reset");

    /// <summary>
    /// Email change purpose.
    /// </summary>
    public static UserVerificationPurpose EmailChange { get; } = new("email_change");

    private UserVerificationPurpose(string value)
    {
        Guard.EnsureStringNotNullOrWhiteSpace(value, nameof(value));
        Value = value.Trim();
    }

    /// <summary>
    /// Returns the raw value for persistence and serialization.
    /// </summary>
    public override string ToString() => Value;
}