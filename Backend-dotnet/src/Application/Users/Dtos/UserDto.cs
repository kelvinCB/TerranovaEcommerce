using Domain.Entities;

namespace Application.Users.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for a user entity.
/// </summary>
public sealed class UserDto
{
    // Properties
    public Ulid Id { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public required DateOnly BirthDate { get; init; }
    public required char Gender { get; init; }
    public required bool IsActive { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }
    public required string EmailAddress { get; init; }
    public required bool IsDeleted { get; init; }

    /// <summary>
    /// Converts a user entity to a UserDto.
    /// </summary>
    /// <param name="user">The user entity to convert.</param>
    /// <returns>Returns a UserDto representation of the user entity.</returns>
    public static UserDto FromDomain(User user) => new UserDto
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        PhoneNumber = user.PhoneNumber?.Value,
        BirthDate = user.BirthDate,
        Gender = user.Gender,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
        EmailAddress = user.EmailAddress.Value,
        IsDeleted = user.IsDeleted
    };
}