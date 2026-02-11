namespace Domain.ValueObjects
{
  public sealed record PasswordHash
  {
    public string Value { get;}

    private PasswordHash(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
       throw new ArgumentException("Password is required.", nameof(value));

      Value = value;
    }

    public static PasswordHash From(string value) => new PasswordHash(value);

    public override string ToString() => "PasswordHash(***)";
  }
}