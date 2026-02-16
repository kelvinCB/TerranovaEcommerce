namespace Domain.ValueObjects
{
  /// <summary>
  /// Represents a password hash as a value object,
  /// ensuring that it is valid and properly formatted according to domain rules.
  /// </summary>
  public sealed record PasswordHash
  {
    /// <summary>
    /// Gets the password hash value. The value is guaranteed to be a non-empty string.
    /// </summary>
    public string Value { get;}

    /// <summary>
    /// Initializes a new instance of the PasswordHash class with the specified password hash value,
    /// validating that it is a non-empty string.
    /// </summary>
    /// <param name="value">The password hash value to be stored.</param>
    /// <exception cref="ArgumentException">Thrown when the password hash is null or empty.</exception>
    private PasswordHash(string hash)
    {
      if (string.IsNullOrWhiteSpace(hash))
       throw new ArgumentException("Password hash is required.", nameof(hash));

      hash = hash.Trim();

      if (hash.Length <= 64)
        throw new ArgumentException("Password hash must be longer than 64 characters.", nameof(hash));

      for (int i = 0; i < hash.Length; i++)
      {
        if (char.IsWhiteSpace(hash[i]))
          throw new ArgumentException("Password hash cannot contain whitespace characters.", nameof(hash));
      }
      
      Value = hash;
    }

    /// <summary>
    /// Creates a new PasswordHash value object from the provided string value,
    /// </summary>
    /// <param name="value">The password hash value to be created.</param>
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