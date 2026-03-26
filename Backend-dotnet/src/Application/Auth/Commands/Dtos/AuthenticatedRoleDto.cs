using Domain.Entities;

namespace Application.Auth.Commands.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for an authenticated role.
/// </summary>
public sealed class AuthenticatedRoleDto
{
    public Ulid Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="AuthenticatedRoleDto"/> class.
    /// </summary>
    /// <param name="role">The role entity to convert.</param>
    /// <returns>Returns a new instance of the <see cref="AuthenticatedRoleDto"/> class.</returns>
    public static AuthenticatedRoleDto FromDomain(Role role) => new()
    {
        Id = role.Id,
        Name = role.Name,
        Description = role.Description
    };
}