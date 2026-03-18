using MediatR;

namespace Application.Users.Commands.SetUserRole;

/// <summary>
/// Represents a command for setting a user's role.
/// </summary>
/// <param name="UserId">The ID of the user to set the role for.</param>
/// <param name="RoleId">The ID of the role to set for the user.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record SetUserRoleCommand(Ulid UserId, Ulid RoleId) : IRequest<Unit>;