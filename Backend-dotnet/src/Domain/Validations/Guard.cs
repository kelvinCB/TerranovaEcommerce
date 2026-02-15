namespace Domain.Validations
{
  public static class Guard
  {
    // Check if Timestamp is not null and is in UTC format, throw an exception if it is not
    public static void EnsureUtc(DateTimeOffset value, string propertyName)
    {
      // Validate that the value is not null
      if (value == default) 
        throw new ArgumentException("Timestamp is required.", propertyName);

      // Validate that the value is in UTC
      if (value.Offset != TimeSpan.Zero)
        throw new ArgumentException("Timestamp must be in UTC (offset 00:00).", propertyName);
    }

    // Check if the string value is null or empty and throw an exception if it is
    public static void EnsureStringNotEmpty(string value, string propertyName)
    {
      if (string.IsNullOrWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be null or whitespace.");
      }
    }

    // Creck if the char value is empty and throw an exception if it is
    public static void EnsureCharNotEmpty(char value, string propertyName)
    {
      if (char.IsWhiteSpace(value))
      {
        throw new ArgumentException($"The property '{propertyName}' cannot be empty.");
      }
    }
  }
}