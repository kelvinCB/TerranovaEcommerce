using Domain.Entities;

namespace Application.Users.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for a role entity.
/// </summary>
public sealed class RoleDto
{
    // Properties
    public Ulid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    /// <summary>
    /// Converts a Role entity to a RoleDto.
    /// </summary>
    /// <param name="role">The Role entity to convert.</param>
    /// <returns>Returns a RoleDto.</returns>
    public static RoleDto FromDomain(Role role) => new RoleDto
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description
    };
}