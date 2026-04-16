using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for user verification-related operations.
/// </summary>
public interface IUserVerificationRepository
{
    /// <summary>
    /// Retrieves an active verification for the specified user, purpose and code.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="purpose">The verification purpose.</param>
    /// <param name="code">The verification code.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The active verification if found; otherwise, null.</returns>
    Task<UserVerification?> GetActiveByUserIdPurposeAndCodeAsync(Ulid userId, UserVerificationPurpose purpose, Code code, CancellationToken cancellationToken);

    /// <summary>
    /// Determines whether an active verification exists for the specified user and purpose.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="purpose">The verification purpose.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if an active verification exists; otherwise, false.</returns>
    Task<bool> ExistsActiveVerificationAsync(Ulid userId, UserVerificationPurpose purpose, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user verification.
    /// </summary>
    /// <param name="userVerification">The user verification to add.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task AddAsync(UserVerification userVerification, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a user verification.
    /// </summary>
    /// <param name="userVerification">The user verification to update.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(UserVerification userVerification, CancellationToken cancellationToken);
}