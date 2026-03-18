namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a user already has a role.
/// </summary>
public sealed class UserAlreadyHasRoleException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserAlreadyHasRoleException"/> class.
  /// </summary>
  /// <param name="userId">The ID of the user that already has a role.</param>
  /// <param name="roleId">The ID of the role that the user already has.</param>
  public UserAlreadyHasRoleException(Ulid userId, Ulid roleId) : base($"User with id '{userId}' already has role with id '{roleId}'") { }
}