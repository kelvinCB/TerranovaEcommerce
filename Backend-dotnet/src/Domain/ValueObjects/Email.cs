namespace Domain.ValueObjects
{
  public sealed record Email
  {
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("The email is required.", nameof(value));

      value = value.Trim().ToLowerInvariant();

      if (!value.Contains("@"))
        throw new ArgumentException("The email format is invalid.", nameof(value));

      return new Email(value);
    }
  }
}