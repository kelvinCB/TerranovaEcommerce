namespace Domain.Validations
{
  public static class Guard
  {
    // Check if Timestamp is not null and is in UTC format, throw an exception if it is not
    public static void EnsureUtc(DateTimeOffset value, string propertyName)
    {
      if (IsUninitialized(value))
        throw new ArgumentException("Timestamp is uninitialized.", propertyName);

      if (value.Offset != TimeSpan.Zero)
        throw new ArgumentException("Timestamp must be in UTC (offset 00:00).", propertyName);
    }

    // Check if the string value is null or whitespace and throw an exception if it is
    public static void EnsureStringNotNullOrWhiteSpace(string value, string propertyName)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be null or whitespace.", propertyName);
      }
    }

    // Check if the char value is uninitialized or it has whitespace and throw an exception if it is
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