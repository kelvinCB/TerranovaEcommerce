namespace Domain.ValueObjects
{
  /// <summary>
  /// Represents a password hash as a value object,
  /// ensuring that it is valid and properly formatted according to domain rules.
  /// </summary>
  public sealed record PasswordHash
  {
    private const int MinimumLength = 65;

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
    /// <exception cref="ArgumentException">Thrown when the provided hash value is not longer than 64 characters.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided hash value contains whitespace characters.</exception>
    private PasswordHash(string hash)
    {
      if (string.IsNullOrWhiteSpace(hash))
        throw new ArgumentException("Password hash is required.", nameof(hash));

      hash = hash.Trim();

      if (hash.Length < MinimumLength)
        throw new ArgumentException($"Password hash must be longer than {MinimumLength} characters.", nameof(hash));

      for (int i = 0; i < hash.Length; i++)
      {
        // Check for whitespace characters in the hash value ej. "test @test.com"
        if (char.IsWhiteSpace(hash[i]))
          throw new ArgumentException("Password hash cannot contain whitespace characters.", nameof(hash));
      }
      
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
  }
}