namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when the activation status of a user is not changed.
/// </summary>
public sealed class UserActivationStatusNotChangedException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UserActivationStatusNotChangedException"/> class.
  /// </summary>
  /// <param name="id">The ID of the user whose activation status was not changed.</param>
  public UserActivationStatusNotChangedException(Ulid id) : base($"Activation status was not changed for user with id '{id}'.") { }
}