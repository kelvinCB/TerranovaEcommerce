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
  /// Checks if a user-role relationship exists.
  /// </summary>
  /// <param name="userId">The ID of the user.</param>
  /// <param name="roleId">The ID of the role.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  /// <returns>Returns a boolean value indicating whether the user-role relationship exists.</returns>
  Task<bool> ExistsUserRoleAsync(Ulid userId, Ulid roleId, CancellationToken cancellationToken);

  /// <summary>
  /// Registers a user-role relationship.
  /// </summary>
  /// <param name="userRole">The user-role relationship to register.</param>
  /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
  Task RegisterAsync(UserRole userRole, CancellationToken cancellationToken);
}