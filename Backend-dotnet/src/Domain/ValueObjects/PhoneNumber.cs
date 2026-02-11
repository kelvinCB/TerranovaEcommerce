using System.Text.RegularExpressions;

namespace Domain.ValueObjects
{
  public sealed record PhoneNumber
  {
    private static readonly Regex PhoneNumberRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

    public string Value { get; }

    private PhoneNumber(string value) => Value = value;

    public static PhoneNumber Create(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("The phone number is required.", nameof(value));

      value = value.Trim();

      if (!PhoneNumberRegex.IsMatch(value))
        throw new ArgumentException("The phone number format is invalid.", nameof(value));

      return new PhoneNumber(value);
    }

    public override string ToString() => Value;
  }
}