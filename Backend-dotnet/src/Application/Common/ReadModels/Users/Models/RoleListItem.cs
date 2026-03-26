namespace Application.Common.ReadModels.Users.Models;

/// <summary>
/// Represents a role list item read model.
/// </summary>
public sealed class RoleListItem
{
    public Ulid Id { get; init; } = Ulid.Empty;
    public required string Name { get; init; }
    public string? Description { get; init; }
}