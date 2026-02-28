using Domain.Validations;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a code value object for user verification.
/// </summary>
public sealed record class Code
{
    /// <summary>
    /// The minimum length allowed for the code.
    /// </summary>
    private const int minimumCodeLength = 6;

    /// <summary>
    /// The value of the code.
    /// </summary>
    private readonly string value;

    /// <summary>
    /// Creates a new instance of the <see cref="Code"/> class with the specified code and performs necessary validations.
    /// </summary>
    /// <param name="code">The code to be stored.</param>
    /// <exception cref="ArgumentException">Thrown when the provided code is null, empty, or contains only whitespace.</exception>
    private Code(string code)
    {
        // Perform validation using the Guard class to ensure domain invariants are maintained
        Guard.EnsureStringNotNullOrWhiteSpace(code, nameof(code));

        code = code.Trim();

        if (code.Length < minimumCodeLength)
            throw new ArgumentException($"The {nameof(code)} must be at least {minimumCodeLength} characters long.");

        CodeHasWhiteSpace(code, nameof(code));

        value = code;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Code"/> class.
    /// </summary>
    /// <param name="code">The code to be stored.</param>
    /// <returns>Returns a new instance of the <see cref="Code"/> class.</returns>
    public static Code From(string code) => new Code(code);

    /// <summary>
    /// Returns a string representation of the code value object.
    /// </summary>
    /// <returns>Returns a string containing the masked code.</returns>
    public override string ToString() => "Code(****)";

    /// <summary>
    /// Checks if the code is equal to the provided code.
    /// </summary>
    /// <param name="code">The code to compare.</param>
    /// <returns>Returns true if the codes are equal, false otherwise.</returns>
    public bool IsMatch(string code) => !string.IsNullOrWhiteSpace(code) && value == code;

    /// <summary>
    /// Checks if the code contains any whitespace characters.
    /// </summary>
    /// <param name="code">The code to check</param>
    /// <param name="propertyName">The name of the property</param>
    /// <exception cref="ArgumentException">Thrown when the code contains whitespace characters.</exception>
    private static void CodeHasWhiteSpace(string code, string propertyName)
    {
        if (code.Any(char.IsWhiteSpace))
        {
            throw new ArgumentException($"{propertyName} cannot contain whitespace characters.", propertyName);
        }
    }
}