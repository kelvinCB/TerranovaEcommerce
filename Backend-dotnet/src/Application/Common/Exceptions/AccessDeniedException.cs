namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when access is denied.
/// </summary>
public sealed class AccessDeniedException : ApplicationExceptionBase
{
  /// <summary>
  /// Initializes a new instance of the <see cref="AccessDeniedException"/> class.
  /// </summary>
    public AccessDeniedException() : base("Access denied.") { }
}