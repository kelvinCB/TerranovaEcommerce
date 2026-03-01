using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a repository for user-related operations.
/// </summary>
public interface IUserRepository
{
    // Queries

    /// <summary>
    /// Retrieves a user by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>Returns the user if found; otherwise, null.</returns>
    Task<User?> GetByIdAsync(Ulid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a user by their email address.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>Returns the user if found; otherwise, null.</returns>
    Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user with the specified email address exists.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>Returns true if the user exists; otherwise, false.</returns>
    Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);

    // Commands

    /// <summary>
    /// Registers a new user in the repository.
    /// </summary>
    /// <param name="user">The user to register.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    Task RegisterAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user in the repository.
    /// </summary>
    /// <param name="user">The user to update.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    Task UpdateAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Soft deletes a user from the repository.
    /// </summary>
    /// <param name="id">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    Task SoftDeleteAsync(Ulid id, CancellationToken cancellationToken = default);
}