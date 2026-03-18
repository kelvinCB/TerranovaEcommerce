using Domain.Entities;

namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for role-related operations.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Checks if a role exists by its ID.
    /// </summary>
    /// <param name="id">The ID of the role to check.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>Returns true if the role exists, otherwise false.</returns>
    Task<bool> ExistsByIdAsync(Ulid id, CancellationToken cancellationToken);
}