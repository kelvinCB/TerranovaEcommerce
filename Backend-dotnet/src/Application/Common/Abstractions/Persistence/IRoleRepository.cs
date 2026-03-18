using Domain.Entities;

namespace Application.Common.Abstractions.Persistence;

public interface IRoleRepository
{
    Task<IReadOnlyCollection<Role>?> GetByUserIdAsync(Ulid userId, CancellationToken cancellationToken);
    Task<bool> ExistsByIdAsync(Ulid id, CancellationToken cancellationToken);
}