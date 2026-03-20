using MediatR;

namespace Application.Users.Commands.AssignRolesToUser;

/// <summary>
/// Represents a command for assigning roles to a user.
/// </summary>
/// <param name="UserId">The ID of the user to assign roles to.</param>
/// <param name="RoleIds">The IDs of the roles to assign to the user.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record AssignRolesToUserCommand(Ulid UserId, IReadOnlyCollection<Ulid> RoleIds) : IRequest<Unit>;