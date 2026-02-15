namespace Domain.Validations
{
  public static class Guard
  {
    // Check if Timestamp is not null and is in UTC format, throw an exception if it is not
    public static void EnsureUtc(DateTimeOffset value, string propertyName)
    {
      // Validate that the value is not null
      if (IsUninitialized(value)) 
        throw new ArgumentException("Timestamp is uninitialized.", propertyName);

      // Validate that the value is in UTC
      if (value.Offset != TimeSpan.Zero)
        throw new ArgumentException("Timestamp must be in UTC (offset 00:00).", propertyName);
    }

    // Check if the string value is null or empty and throw an exception if it is
    public static void EnsureStringNotEmpty(string value, string propertyName)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be null or whitespace.", propertyName);
      }
    }

    // Check if the char value is empty and throw an exception if it is
    public static void EnsureCharInitializedAndNotWhiteSpace(char value, string propertyName)
    {
      if (char.IsWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be whitespace.", propertyName);
      }

      if (IsUninitialized(value))
      {
        throw new ArgumentException($"The property '{propertyName}' is uninitialized.", propertyName);
      }
    }

    // Check if the value is default and throw false if it is
    private static bool IsUninitialized<T>(T value) => EqualityComparer<T>.Default.Equals(value, default);
  }
}