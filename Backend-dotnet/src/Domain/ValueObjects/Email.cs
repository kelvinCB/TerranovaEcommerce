using System.Net.Mail;

namespace Domain.ValueObjects
{
  /// <summary>
  /// Represents an email address as a value object,
  /// ensuring that it is valid and properly formatted according to domain rules.
  /// </summary>
  public sealed record Email
  {
    /// <summary>
    /// Gets the email address value. The value is guaranteed to be a valid email format and is stored in lowercase.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the Email class with the specified email address value.
    /// </summary>
    /// <param name="value">The email address value to be stored.</param>
    private Email(string value) => Value = value;

    /// <summary>
    /// Creates a new Email value object from the provided string value,
    /// validating that it is a properly formatted email address.
    /// </summary>
    /// <param name="value">The email address value to be validated and stored.</param>
    /// <returns>A new Email value object.</returns>
    /// <exception cref="ArgumentException">Thrown when the email is null, empty, or not properly formatted.</exception>
    public static Email Create(string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("The email is required.", nameof(value));

      value = value.Trim().ToLowerInvariant();

      if (value.Contains(' '))
        throw new ArgumentException("The email cannot contain spaces.", nameof(value));

      int at = value.IndexOf('@');
      if (at <= 0 || at != value.LastIndexOf('@') || at == value.Length - 1)
        throw new ArgumentException("The email format is invalid.", nameof(value));

      try
      {
        var mailAddress = new MailAddress(value);

        // Must match exactly; avoids some odd normalizations.
        if (mailAddress.Address != value)
          throw new ArgumentException("The email format is invalid.", nameof(value));
      }
      catch (FormatException)
      {
        throw new ArgumentException("The email format is invalid.", nameof(value));
      }

      return new Email(value);
    }
  }
}