using MediatR;

namespace Application.Users.Commands.SoftDeleteUser;

/// <summary>
/// Represents a command for soft deleting a user.
/// </summary>
/// <param name="Id">The ID of the user to soft delete.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record SoftDeleteUserCommand(Ulid Id) : IRequest<Unit>;