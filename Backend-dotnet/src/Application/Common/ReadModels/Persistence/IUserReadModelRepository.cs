using Application.Common.Pagination;
using Application.Common.ReadModels.Users.Models;

namespace Application.Common.ReadModels.Persistence;

/// <summary>
/// Represents a user read model repository.
/// </summary>
public interface IUserReadModelRepository
{
    /// <summary>
    /// Gets a paged list of users.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="search">The search term.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>Returns a PagedResult of UserListItem.</returns>
    Task<PagedResult<UserListItem>> GetPagedAsync(int pageNumber, int pageSize, string? search, CancellationToken cancellationToken);
}