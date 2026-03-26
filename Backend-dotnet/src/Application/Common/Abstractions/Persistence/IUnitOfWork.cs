namespace Application.Common.Abstractions.Persistence;

/// <summary>
/// Represents a unit of work for database operations.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Saves changes to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}