using MediatR;

namespace Application.Auth.Commands.ResetPassword;

/// <summary>
/// Represents a command to reset a user's password using a password reset code.
/// </summary>
/// <param name="Email">The email address of the user.</param>
/// <param name="Code">The password reset code.</param>
/// <param name="NewPassword">The new password for the user.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record ResetPasswordCommand(
    string Email,
    string Code,
    string NewPassword
) : IRequest<Unit>;