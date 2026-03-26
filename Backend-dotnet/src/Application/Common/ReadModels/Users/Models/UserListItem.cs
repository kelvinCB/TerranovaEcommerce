namespace Application.Common.ReadModels.Users.Models;

/// <summary>
/// Represents a user list item read model.
/// </summary>
public sealed class UserListItem
{
    public Ulid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string EmailAddress { get; init; }
    public string? PhoneNumber { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public required IReadOnlyCollection<RoleListItem> Roles { get; init; }
}