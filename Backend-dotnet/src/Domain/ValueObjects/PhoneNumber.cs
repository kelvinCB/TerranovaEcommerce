using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a phone number value object.
/// </summary>
public sealed record PhoneNumber
{
    /// <summary>
    /// Regular expression to validate international phone numbers in E.164 format.
    /// </summary>
    /// <remarks>
    /// The E.164 format allows for a maximum of 15 digits and an optional leading '+' sign.
    /// Examples of valid phone numbers: +18298091212, 18298091212
    /// </remarks>
    private static readonly Regex PhoneNumberRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Gets the phone number value.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneNumber"/> record with the specified value.
    /// </summary>
    /// <param name="value">The phone number value to be stored.</param>
    private PhoneNumber(string value) => Value = value;

    /// <summary>
    /// Creates a new instance of the <see cref="PhoneNumber"/> record from the specified string value.
    /// </summary>
    /// <param name="value">The phone number value to be created.</param>
    /// <returns>A new PhoneNumber value object.</returns>
    /// <exception cref="ArgumentException">Thrown when the phone number is null, empty, or has an invalid format.</exception>
    public static PhoneNumber Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("The phone number is required.", nameof(value));

        value = value.Trim();

        if (!PhoneNumberRegex.IsMatch(value))
            throw new ArgumentException("The phone number format is invalid.", nameof(value));

        // Normalize to always include the Leading '+' sign for consistency, if it's not already present.
        if (!value.StartsWith("+"))
            value = "+" + value;

        return new PhoneNumber(value);
    }

    /// <summary>
    /// Returns a string representation of the phone number value object.
    /// </summary>
    /// <returns>The phone number value as a string.</returns>
    /// <remarks> This method returns the raw phone number value without any formatting. 
    /// The value is expected to be in E.164 format, which may include a leading '+' sign and up to 15 digits. 
    /// </remarks>
    public override string ToString() => Value;
}
