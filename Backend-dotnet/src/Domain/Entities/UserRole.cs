using Domain.Validations;

namespace Domain.Entities;

/// <summary>
/// Represents a user-role assignment.
/// </summary>
public class UserRole
{
    public Ulid UserId { get; private set; }
    public Ulid RoleId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the UserRole class.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="createdAt">The timestamp when the user-role assignment was created.</param>
    private UserRole(Ulid userId, Ulid roleId, DateTimeOffset createdAt)
    {
        UserId = userId;
        RoleId = roleId;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new instance of the UserRole class and performs necessary validations.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="timestamp">The timestamp when the user-role assignment was created.</param>
    /// <returns>A new instance of the UserRole class.</returns>
    public static UserRole Create(Ulid userId, Ulid roleId, DateTimeOffset timestamp)
    {
        // Perform validations using the Guard class to ensure domain invariants are maintained
        Guard.EnsureUlidNotEmpty(userId, nameof(userId));
        Guard.EnsureUlidNotEmpty(roleId, nameof(roleId));
        Guard.EnsureUtc(timestamp, nameof(timestamp));

        return new UserRole(
            userId,
            roleId,
            timestamp
        );
    }
    
}