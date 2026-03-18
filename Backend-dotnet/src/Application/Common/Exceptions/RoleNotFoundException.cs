namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when a role is not found.
/// </summary>
public sealed class RoleNotFoundException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RoleNotFoundException"/> class.
    /// </summary>
    /// <param name="id">The ID of the role that was not found.</param>
    public RoleNotFoundException(Ulid id) : base($"Role with id '{id}' was not found.") { }
}