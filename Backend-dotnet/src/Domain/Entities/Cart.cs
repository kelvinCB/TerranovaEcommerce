using Domain.Validations;

namespace Domain.Entities;

/// <summary>
/// Represents a shopping cart associated with a user.
/// Each cart has a unique identifier, a reference to the user it belongs to,
/// and timestamps for when it was created and last updated.
/// </summary>
public class Cart
{
    public Ulid Id { get; private set; }
    public Ulid UserId { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    /// <summary>
    /// Initializes a new instance of the Cart class with the specified parameters.
    /// </summary>
    /// <param name="id">The cart identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="createdAt">The timestamp for when the cart was created.</param>
    /// <param name="updatedAt">The timestamp for when the cart was last updated.</param>
    private Cart(Ulid id, Ulid userId, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        Id = id;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Creates a new instance of the Cart class with the specified parameters.
    /// </summary>
    /// <param name="id">The cart identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <param name="timestamp">The timestamp for when the cart was created.</param>
    /// <returns>A new instance of the Cart class.</returns>
    /// <exception cref="ArgumentException">Thrown when the cart identifier or user identifier is empty</exception>
    /// <exception cref="ArgumentException">Thrown when the timestamp is not in UTC</exception>
    public static Cart Create
    (
        Ulid id,
        Ulid userId,
        DateTimeOffset timestamp
    )
    {
        Guard.EnsureUlidNotEmpty(id, nameof(id));
        Guard.EnsureUlidNotEmpty(userId, nameof(userId));

        Guard.EnsureUtc(timestamp, nameof(timestamp));

        return new Cart(
            id, 
            userId, 
            createdAt: timestamp,
            updatedAt: timestamp
        );
    }

    /// <summary>
    /// Updates the cart's UpdatedAt timestamp. 
    /// The new timestamp must be in UTC and cannot be before the <see cref="CreatedAt"/> timestamp.
    /// </summary>
    /// <param name="timestamp">The new timestamp for updating the cart.</param>
    /// <exception cref="ArgumentException">Thrown when the timestamp is not in UTC</exception>
    /// <exception cref="ArgumentException">Thrown when the timestamp is before the <see cref="CreatedAt"/> timestamp</exception>
    /// <remarks>
    /// Allows updating the cart only when the <see cref="UpdatedAt"/>
    /// timestamp is after to the <see cref="CreatedAt"/> timestamp.
    /// </remarks>
    public void Update(DateTimeOffset timestamp)
    {
        Guard.EnsureUtc(timestamp, nameof(timestamp));

        Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

        UpdatedAt = timestamp;
    }
}
