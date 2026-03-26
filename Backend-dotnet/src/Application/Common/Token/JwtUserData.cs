using Domain.Entities;

namespace Application.Common.Token;

/// <summary>
/// Represents JWT user data.
/// </summary>
public sealed class JwtUserData
{
    public required Ulid UserId { get; init; }
    public required string EmailAddress { get; init; }
    public required IReadOnlyCollection<string> Roles { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="JwtUserData"/> class.
    /// </summary>
    /// <param name="user">The user entity to convert.</param>
    /// <param name="roles">The user's roles to convert.</param>
    /// <returns>Returns a new instance of the <see cref="JwtUserData"/> class</returns>
    public static JwtUserData Create(User user, IReadOnlyCollection<Role> roles) => new()
    {
        UserId = user.Id,
        EmailAddress = user.EmailAddress.Value,
        Roles = roles.Select(x => x.Name).ToArray()
    };
}