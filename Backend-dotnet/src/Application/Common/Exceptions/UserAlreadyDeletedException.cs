namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a user is already deleted.
/// </summary>
public sealed class UserAlreadyDeletedException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserAlreadyDeletedException"/> class.
  /// </summary>
  /// <param name="id">The ID of the user that was already deleted.</param>
  public UserAlreadyDeletedException(Ulid id) : base($"User with id {id} was already deleted.") { }
}