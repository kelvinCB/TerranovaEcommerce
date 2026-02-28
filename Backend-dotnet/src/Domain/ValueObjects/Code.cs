using Domain.Validations;

namespace Domain.ValueObjects;

/// <summary>
/// Represents a code value object for user verification.
/// </summary>
public sealed record class Code
{
    /// <summary>
    /// The maximum length of the code.
    /// </summary>
    private const int minimumCodeLength = 6;

    /// <summary>
    /// The value of the code.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="Code"/> class with the specified code and performs necessary validations.
    /// </summary>
    /// <param name="code">The code to be stored.</param>
    /// <exception cref="ArgumentException">Thrown when the provided code is null, empty, or contains only whitespace.</exception>
    private Code (string code)
    {
        // Perform validation using the Guard class to ensure domain invariants are maintained
        Guard.EnsureStringNotNullOrWhiteSpace(code, nameof(code));

        code = code.Trim();

        if (code.Length < minimumCodeLength)
            throw new ArgumentException($"The {nameof(code)} must be at least {minimumCodeLength} characters.");

        CodeHasWhiteSpace(code, nameof(code));

        Value = code;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Code"/> class.
    /// </summary>
    /// <param name="code">The code to be stored.</param>
    /// <returns>Returns a new instance of the <see cref="Code"/> class.</returns>
    public static Code From(string code) => new Code(code);

    public override string ToString() => "Code(****)";

    /// <summary>
    /// Checks if the code contains any whitespace characters.
    /// </summary>
    /// <param name="code">The code to check</param>
    /// <param name="propertyName">The name of the property</param>
    /// <exception cref="ArgumentException">Thrown when the code contains whitespace characters.</exception>
    private static void CodeHasWhiteSpace(string code, string propertyName)
    {
        for (int i = 0; i < code.Length; i++)
        {
            if (char.IsWhiteSpace(code[i]))
            {
                throw new ArgumentException($"{propertyName} cannot contain whitespace characters.", propertyName);
            }
        }
    }
}