namespace Domain.Validations
{
  /// <summary>
  /// Provides guard clauses for validating method arguments and properties.
  /// </summary>
  public static class Guard
  {
    /// <summary>
    /// Ensures that a DateTimeOffset value is initialized and in UTC format.
    /// </summary>
    /// <param name="value">The DateTimeOffset value to validate.</param>
    /// <param name="propertyName">The name of the property being validated.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is uninitialized or not in UTC format.
    /// </exception>
    public static void EnsureUtc(DateTimeOffset value, string propertyName)
    {
      if (IsUninitialized(value))
        throw new ArgumentException("Timestamp is uninitialized.", propertyName);

      if (value.Offset != TimeSpan.Zero)
        throw new ArgumentException("Timestamp must be in UTC (offset 00:00).", propertyName);
    }

    /// <summary>
    /// Ensures that a DateTimeOffset value is not before a specified reference DateTimeOffset value.
    /// </summary>
    /// <param name="value">The DateTimeOffset value to validate.</param>
    /// <param name="reference">The reference DateTimeOffset value.</param>
    /// <param name="propertyName">The name of the property being validated.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is before the reference value.
    /// </exception>
    public static void EnsureUtcNotBefore(DateTimeOffset value, DateTimeOffset reference, string propertyName)
    {
      EnsureUtc(value, propertyName); // Ensure the value is in UTC before comparing

      if (value < reference)
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be before {reference}.", propertyName);
      }
    }

    /// <summary>
    /// Ensures that a string value is not null, empty, or consists only of whitespace characters.
    /// </summary>
    /// <param name="value">The string value to validate.</param>
    /// <param name="propertyName">The name of the property being validated.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is null, empty, or consists only of whitespace characters.
    /// </exception>
    public static void EnsureStringNotNullOrWhiteSpace(string value, string propertyName)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be null or whitespace.", propertyName);
      }
    }

    /// <summary>
    /// Ensures that a char value is initialized and not a whitespace character.
    /// </summary>
    /// <param name="value">The char value to validate.</param>
    /// <param name="propertyName">The name of the property being validated.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the value is uninitialized or is a whitespace character.
    /// </exception>
    public static void EnsureCharInitializedAndNotWhiteSpace(char value, string propertyName)
    {
      if (IsUninitialized(value))
      {
        throw new ArgumentException($"The property '{propertyName}' is uninitialized.", propertyName);
      }

      if (char.IsWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be whitespace.", propertyName);
      }
    }

    // Check if the value is default and return true if it is
    private static bool IsUninitialized<T>(T value) => EqualityComparer<T>.Default.Equals(value, default);
  }
}