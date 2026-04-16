namespace Application.Common.Abstractions.Services;

/// <summary>
/// Defines an interface for notification services, providing methods to send various types of notifications to users.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends a password reset code to the specified email address, allowing the user to reset their password.
    /// </summary>
    /// <param name="email">The email address to send the password reset code to.</param>
    /// <param name="code">The password reset code to send.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendPasswordResetCodeToEmailAsync(string email, string code, CancellationToken cancellationToken);

    /// <summary>
    /// Sends an email verification code to the specified email address, allowing the user to verify their email address.
    /// </summary>
    /// <param name="email">The email address to send the verification code to.</param>
    /// <param name="code">The email verification code to send.</param>
    /// <param name="cancellationToken">A cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SendEmailVerificationCodeToEmailAsync(string email, string code, CancellationToken cancellationToken);
}