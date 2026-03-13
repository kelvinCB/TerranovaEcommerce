using MediatR;

namespace Application.Users.Commands.SetUserActivationStatus;

/// <summary>
/// Represents a command for changing the activation status of a user.
/// </summary>
/// <param name="Id">The ID of the user to change the activation status for.</param>
/// <param name="IsActive">The new activation status for the user.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record SetUserActivationStatusCommand(Ulid Id, bool IsActive) : IRequest<Unit>;