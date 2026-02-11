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

            EnsureUtc(timestamp);

            Id = id;
            UserId = userId;
            CreatedAt = timestamp;
            UpdatedAt = timestamp;
        }

        public void UpdateCart(DateTimeOffset timestamp)
        {
            EnsureUtc(timestamp);

            UpdatedAt = timestamp;
        }

        private static void EnsureUtc(DateTimeOffset value)
        {
            // Validate that the value is not null
            if (value == default) 
                throw new ArgumentException("Timestamp is required.", nameof(value));

            // Validate that the value is in UTC
            if (value.Offset != TimeSpan.Zero)
                throw new ArgumentException("Timestamp must be in UTC (offset 00:00).", nameof(value));
        }
    }
}