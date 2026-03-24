namespace Application.Common.Exceptions;

/// <summary>
/// Thrown when at least one role must be provided.
/// </summary>
public sealed class AtLeastOneRoleMustBeProvidedException : ApplicationExceptionBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtLeastOneRoleMustBeProvidedException"/> class.
    /// </summary>
    public AtLeastOneRoleMustBeProvidedException() : base("At least one role must be provided.") { }

    /// <summary>
    /// Throws an exception if the role IDs are null or empty.
    /// </summary>
    /// <param name="roleIds">The role IDs.</param>
    public static void ThrowIfNullOrEmpty(IReadOnlyCollection<Ulid>? roleIds)
    {
        if (roleIds == null || roleIds.Count == 0)
        {
            throw new AtLeastOneRoleMustBeProvidedException();
        }
    }
}