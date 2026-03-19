namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a user must have at least one role.
/// </summary>
public sealed class UserMustHaveAtLeastOneRoleException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserMustHaveAtLeastOneRoleException"/> class.
  /// </summary>
  public UserMustHaveAtLeastOneRoleException() : base($"User must have at least one role.") { }

  /// <summary>
  /// Throws an exception if the role IDs are null or empty.
  /// </summary>
  /// <param name="roleIds">The role IDs.</param>
  public static void ThrowIfNullOrEmpty(IReadOnlyCollection<Ulid>? roleIds)
  {
    if (roleIds == null || roleIds.Count == 0)
    {
      throw new UserMustHaveAtLeastOneRoleException();
    }
  }
}