namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a user has no roles.
/// </summary>
public sealed class UserHasNoRoleException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserHasNoRoleException"/> class.
    /// </summary>
    /// <param name="id">The ID of the user that has no roles.</param>
    public UserHasNoRoleException(Ulid id) : base($"User with id {id} has no role.") { }
}