namespace Application.Common.Exceptions;

/// <summary>
/// Exception thrown when a refresh token is not active.
/// </summary>
public sealed class RefreshTokenNotActiveException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenNotActiveException"/> class with a default error message.
    /// </summary>
    public RefreshTokenNotActiveException() : base("Refresh token is not active.") { }
}