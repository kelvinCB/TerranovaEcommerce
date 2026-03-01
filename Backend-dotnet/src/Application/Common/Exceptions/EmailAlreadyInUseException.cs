namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when an email is already in use.
/// </summary>
public sealed class EmailAlreadyInUseException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailAlreadyInUseException"/> class.
    /// </summary>
    /// <param name="email">The email that is already in use.</param>
    public EmailAlreadyInUseException(string email) : base($"Email '{email}' is already in use.") { }
}