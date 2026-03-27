using Domain.Entities;

namespace Application.Auth.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for an authenticated user.
/// </summary>
public sealed class AuthenticatedUserDto
{
    public required Ulid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required IReadOnlyCollection<AuthenticatedRoleDto> Roles { get; init; } = [];

    /// <summary>
    /// Creates a new instance of the <see cref="AuthenticatedUserDto"/> class.
    /// </summary>
    /// <param name="user">The user entity to convert.</param>
    /// <param name="roles">The user's roles</param>
    /// <returns>Returns a new instance of the <see cref="AuthenticatedUserDto"/> class</returns>
    public static AuthenticatedUserDto FromDomain(User user, IReadOnlyCollection<Role> roles) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.EmailAddress.Value,
        Roles = roles.Select(x => AuthenticatedRoleDto.FromDomain(x)).ToArray()
    };
}