namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for role-related operations.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets existing role IDs.
    /// </summary>
    /// <param name="roleIds">The IDs of the roles to check.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Returns a collection of existing role IDs.</returns>
    Task<IReadOnlyCollection<Ulid>> GetExistingRoleIdsAsync(IReadOnlyCollection<Ulid> roleIds, CancellationToken cancellationToken);
}