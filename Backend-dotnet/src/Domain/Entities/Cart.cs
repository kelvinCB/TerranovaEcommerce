namespace Domain.Entities
{
    public class Cart
    {
        public Ulid Id { get; private set; }
        public Ulid UserId { get; private set; }
        public DateTimeOffset CreatedAt { get; private set; }
        public DateTimeOffset UpdatedAt { get; private set; }

        public Cart(Ulid id, Ulid userId, DateTimeOffset utcNow)
        {
            if (id == Ulid.Empty) throw new ArgumentException("Id is required.", nameof(id));
            if (userId == Ulid.Empty) throw new ArgumentException("User Id is required.", nameof(userId));
            if (utcNow.Offset != TimeSpan.Zero) throw new ArgumentException("CreatedAt must be in UTC.", nameof(utcNow));

            Id = id;
            UserId = userId;
            CreatedAt = utcNow;
            UpdatedAt = utcNow;
        }

        public void UpdateCart(DateTimeOffset utcNow)
        {
            if (utcNow.Offset != TimeSpan.Zero) throw new ArgumentException("UpdatedAt must be in UTC.", nameof(utcNow));

            UpdatedAt = utcNow;
        }
    }
}