using Domain.Validations;

namespace Domain.Entities
{
    public class Cart
    {
        public Ulid Id { get; private set; }
        public Ulid UserId { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        public Cart(Ulid id, Ulid userId, DateTimeOffset timestamp)
        {
            if (id == Ulid.Empty) throw new ArgumentException("Id is required.", nameof(id));
            if (userId == Ulid.Empty) throw new ArgumentException("User Id is required.", nameof(userId));

            Guard.EnsureUtc(timestamp, nameof(timestamp));

            Id = id;
            UserId = userId;
            CreatedAt = timestamp;
            UpdatedAt = timestamp;
        }

        public void UpdateCart(DateTimeOffset timestamp)
        {
            Guard.EnsureUtc(timestamp, nameof(timestamp));

            if (timestamp < CreatedAt)
                throw new ArgumentException("UpdatedAt cannot be before CreatedAt.", nameof(timestamp));

            UpdatedAt = timestamp;
        }
    }
}