namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a user is not found.
/// </summary>
public sealed class UserNotFoundException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserNotFoundException"/> class.
  /// </summary>
  /// <param name="id">The ID of the user that was not found.</param>
  public UserNotFoundException(Ulid id) : base($"User with id {id} was not found.") {}
}