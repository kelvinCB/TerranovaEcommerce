using Domain.Validations;

namespace Domain.Entities;

/// <summary>
/// Represents a role in the system with properties such as a name, description
/// and timestamp for when it was created.
/// </summary>
public class Role
{
    public Ulid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Role"/> class with the specified parameters.
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="name">The name of the role</param>
    /// <param name="description">The description of the role</param>
    /// <param name="createdAt">The timestamp when the role was created</param>
    private Role(
        Ulid id,
        string name,
        DateTimeOffset createdAt,
        string? description
    )
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Role"/> class with the specified parameters and performs necessary validations.
    /// </summary>
    /// <param name="id">The role identifier</param>
    /// <param name="name">The name of the role</param>
    /// <param name="description">The description of the role</param>
    /// <param name="timestamp">The timestamp when the role was created</param>
    /// <returns>The newly created <see cref="Role"/> instance</returns>
    public static Role Create(
        Ulid id,
        string name,
        string? description,
        DateTimeOffset timestamp
    )
    {
        Guard.EnsureUlidNotEmpty(id, nameof(id));
        Guard.EnsureStringNotNullOrWhiteSpace(name, nameof(name));
        Guard.EnsureUtc(timestamp, nameof(timestamp));

        return new Role
        (
            id,
            name,
            createdAt: timestamp,
            description
        );
    }

    /// <summary>
    /// Sets the description of the role
    /// </summary>
    /// <param name="description">The description of the role</param>
    public void SetDescription(string? description) => Description = description;

    /// <summary>
    /// Sets the name of the role
    /// </summary>
    /// <param name="name">The name of the role</param>
    public void SetName(string name)
    {
        Guard.EnsureStringNotNullOrWhiteSpace(name, nameof(name));

        Name = name;
    }
}