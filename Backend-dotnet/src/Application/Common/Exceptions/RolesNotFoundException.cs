namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when roles are not found.
/// </summary>
public sealed class RolesNotFoundException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolesNotFoundException"/> class.
    /// </summary>
    /// <param name="roleIds">The IDs of the roles that were not found.</param>
    public RolesNotFoundException(IReadOnlyCollection<Ulid> roleIds) : base($"Roles with IDs {string.Join(", ", roleIds)} were not found.") { }
}