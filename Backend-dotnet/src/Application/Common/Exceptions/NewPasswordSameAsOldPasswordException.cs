namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when the new password provided by the user is the same as the old password during a password reset operation.
/// </summary>
public sealed class NewPasswordSameAsOldPasswordException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NewPasswordSameAsOldPasswordException"/> class.
    /// </summary>
    public NewPasswordSameAsOldPasswordException() : base("The new password must be different from the current password.") { }
}