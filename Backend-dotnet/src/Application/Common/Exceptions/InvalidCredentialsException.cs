namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when invalid credentials are provided.
/// </summary>
public sealed class InvalidCredentialsException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
  /// </summary>
  public InvalidCredentialsException() : base("Invalid credentials.") { }
}