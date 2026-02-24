namespace Domain.ValueObjects;

/// <summary>
/// Represents a password hash as a value object,
/// ensuring that it is valid and properly formatted according to domain rules.
/// </summary>
public sealed record PasswordHash
{
    /// <summary>
    /// Defines the minimum length requirement for a valid password hash, which must be longer than 63 characters.
    /// </summary>
    private const int MinimumPasswordHashLength = 64;

    /// <summary>
    /// Gets the password hash value. The value is guaranteed to be a non-empty string.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the PasswordHash class with the specified password hash value,
    /// validating that it is a non-empty string.
    /// </summary>
    /// <param name="hash">The password hash value to be stored.</param>
    /// <exception cref="ArgumentException">Thrown when the provided hash value is null, empty, or contains only whitespace.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided hash value is not longer than 63 characters.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided hash value contains whitespace characters.</exception>
    private PasswordHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentException("Password hash is required.", nameof(hash));

        hash = hash.Trim();

        if (hash.Length < MinimumPasswordHashLength)
            throw new ArgumentException($"Password hash must be at least {MinimumPasswordHashLength} characters.", nameof(hash));

        EnsurePasswordHashNotContainsWhitespace(hash);

        Value = hash;
    }

    /// <summary>
    /// Creates a new PasswordHash value object from the provided string value.
    /// </summary>
    /// <param name="hash">The password hash value to be created.</param>
    /// <returns>A new PasswordHash value object.</returns>
    public static PasswordHash From(string hash) => new PasswordHash(hash);

    /// <summary>
    /// Returns a string representation of the PasswordHash value object,
    /// masking the actual hash value for security reasons.
    /// </summary>
    /// <returns>A masked string representation of the password hash.</returns>
    public override string ToString() => "PasswordHash(***)";

    /// <summary>
    /// Validates that the provided password hash value does not contain any whitespace characters.
    /// </summary>
    /// <param name="hash">The password hash value to be validated.</param>
    /// <exception cref="ArgumentException">Thrown when the provided hash value contains whitespace characters.</exception>
    /// <example>"p@ssTest 123 4" is an example of a password hash that contains whitespace and would be considered invalid.</example>
    private static void EnsurePasswordHashNotContainsWhitespace(string hash)
    {
        for (int i = 0; i < hash.Length; i++)
        {
            if (char.IsWhiteSpace(hash[i]))
                throw new ArgumentException("Password hash cannot contain whitespace characters.", nameof(hash));
        }
    }
}
