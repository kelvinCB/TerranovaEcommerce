using MediatR;

namespace Application.Users.Commands.RemoveRolesFromUser;

/// <summary>
/// Represents a command for removing roles from a user.
/// </summary>
/// <param name="UserId">The ID of the user to remove roles from.</param>
/// <param name="RoleIds">The IDs of the roles to remove from the user.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record RemoveRolesFromUserCommand(Ulid UserId, IReadOnlyCollection<Ulid> RoleIds) : IRequest<Unit>;