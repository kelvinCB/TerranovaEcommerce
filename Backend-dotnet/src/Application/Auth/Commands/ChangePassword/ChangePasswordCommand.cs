using MediatR;

namespace Application.Auth.Commands.ChangePassword;

/// <summary>
/// Command to change the password of a user.
/// </summary>
/// <param name="UserId">The ID of the user whose password is to be changed.</param>
/// <param name="CurrentPassword">The current password of the user.</param>
/// <param name="NewPassword">The new password for the user.</param>
/// <remarks>Mediator pattern is used to handle the command and return Unit.</remarks>
public sealed record ChangePasswordCommand(
    Ulid UserId,
    string CurrentPassword,
    string NewPassword
) : IRequest<Unit>;