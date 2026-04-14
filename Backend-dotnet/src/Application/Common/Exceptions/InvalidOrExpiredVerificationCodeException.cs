namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when the verification code is invalid or has expired.
/// </summary>
public sealed class InvalidOrExpiredVerificationCodeException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidOrExpiredVerificationCodeException"/> class.
    /// </summary>
    public InvalidOrExpiredVerificationCodeException() : base("The reset code is invalid or expired.") { }
}