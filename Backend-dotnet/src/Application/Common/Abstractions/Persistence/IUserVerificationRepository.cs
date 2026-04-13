using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for user verification-related operations.
/// </summary>
public interface IUserVerificationRepository
{
    /// <summary>
    /// Retrieves a user verification record by user ID and verification purpose.
    /// </summary>
    /// <param name="userId">The ID of the user for whom to retrieve verification information.</param>
    /// <param name="purpose">The purpose of the verification to retrieve.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The user verification record if found, otherwise null.</returns>
    Task<UserVerification?> GetByUserIdAndTypeAsync(Ulid userId, UserVerificationPurpose purpose, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if there is an active verification for a user with the specified purpose.
    /// </summary>
    /// <param name="userId">The ID of the user to check.</param>
    /// <param name="purpose">The purpose of the verification to check.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>Returns true if an active verification exists, false otherwise.</returns>
    Task<bool> ExistsActiveVerificationAsync(Ulid userId, UserVerificationPurpose purpose, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user verification record to the repository.
    /// </summary>
    /// <param name="userVerification">The user verification record to add.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    Task AddAsync(UserVerification userVerification, CancellationToken cancellationToken);

    /// <summary>
    /// Updates an existing user verification record in the repository.
    /// </summary>
    /// <param name="userVerification">The user verification record to update.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateAsync(UserVerification userVerification, CancellationToken cancellationToken);
}