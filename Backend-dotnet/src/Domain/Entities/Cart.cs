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
    /// <exception cref="ArgumentException">Thrown when the id is empty</exception>
    /// <exception cref="ArgumentException">Thrown when the userId is empty</exception>
    /// <exception cref="ArgumentException">Thrown when the timestamp is not in UTC</exception>
    public static Cart Create
    (
        Ulid id,
        Ulid userId,
        DateTimeOffset timestamp
    )
    {
        if (id == Ulid.Empty) throw new ArgumentException("Id is required.", nameof(id));
        if (userId == Ulid.Empty) throw new ArgumentException("User Id is required.", nameof(userId));

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
    /// <exception cref="InvalidOperationException">Thrown when the cart's CreatedAt timestamp is not set.</exception>
    /// <exception cref="ArgumentException">Thrown when the new timestamp is before the CreatedAt timestamp.</exception>
    public void UpdateCart(DateTimeOffset timestamp)
    {
        Guard.EnsureUtc(timestamp, nameof(timestamp));

        if (CreatedAt == default)
            throw new InvalidOperationException("CreatedAt must be set before updating the cart.");

        if (timestamp < CreatedAt)
            throw new ArgumentException("UpdatedAt cannot be before CreatedAt.", nameof(timestamp));

        UpdatedAt = timestamp;
    }
}
