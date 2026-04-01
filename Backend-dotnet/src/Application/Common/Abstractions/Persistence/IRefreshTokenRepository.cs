using Domain.Entities;

namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for refresh token-related operations.
/// </summary>
public interface IRefreshTokenRepository
{
    /// <summary>
    /// Adds a refresh token to the repository.
    /// </summary>
    /// <param name="refreshToken">The refresh token to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task AddAsync(RefreshToken refreshToken, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a refresh token by its token hash.
    /// </summary>
    /// <param name="tokenHash">The token hash of the refresh token to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The refresh token if found; otherwise, null.</returns>
    Task<RefreshToken?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing refresh token in the repository.
    /// </summary>
    /// <param name="refreshToken">The refresh token to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task UpdateAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
}