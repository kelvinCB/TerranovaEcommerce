using Domain.Entities;

namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for user role-related operations.
/// </summary>
public interface IUserRoleRepository
{
  /// <summary>
  /// Retrieves user roles by user ID.
  /// </summary>
  /// <param name="userId">The ID of the user to retrieve roles for.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  /// <returns>Returns a collection of roles associated with the user.</returns>
  Task<IReadOnlyCollection<Role>> GetByUserIdAsync(Ulid userId, CancellationToken cancellationToken);

  /// <summary>
  /// Retrieves the IDs of roles assigned to a user.
  /// </summary>
  /// <param name="userId">The ID of the user to retrieve role IDs for.</param>
  /// <param name="roleIds">The IDs of the roles to retrieve.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  /// <returns>Returns a collection of role IDs associated with the user.</returns>
  Task<IReadOnlyCollection<Ulid>> GetAssignedRoleIdsAsync(Ulid userId, IReadOnlyCollection<Ulid> roleIds, CancellationToken cancellationToken);

  /// <summary>
  /// Retrieves the IDs of roles assigned to a user.
  /// </summary>
  /// <param name="userId">The ID of the user to retrieve role IDs for.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  /// <returns>Returns a collection of role IDs associated with the user.</returns>
  Task<IReadOnlyCollection<Ulid>> GetRoleIdsByUserIdAsync(Ulid userId, CancellationToken cancellationToken);

  /// <summary>
  /// Assigns roles to a user.
  /// </summary>
  /// <param name="userId">The ID of the user to assign roles to.</param>
  /// <param name="roleIds">The IDs of the roles to assign to the user.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  Task AssignRolesToUserAsync(Ulid userId, IReadOnlyCollection<Ulid> roleIds, CancellationToken cancellationToken);

  /// <summary>
  /// Removes roles from a user.
  /// </summary>
  /// <param name="userId">The ID of the user to remove roles from.</param>
  /// <param name="roleIds">The IDs of the roles to remove from the user.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  Task RemoveRolesFromUserAsync(Ulid userId, IReadOnlyCollection<Ulid> roleIds, CancellationToken cancellationToken);
}