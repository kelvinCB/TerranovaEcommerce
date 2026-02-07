namespace Domain.Entities
{
    public class Cart
    {
        public Ulid Id { get; private set; }
        public Ulid UserId { get; private set; }
        public DateTimeOffset CreateAt { get; private set; }
        public DateTimeOffset UpdateAt { get; private set; }

        public Cart(Ulid userId)
        {
            if(userId == Ulid.Empty) throw new ArgumentException("User Id is required.", nameof(userId));

            Id = Ulid.NewUlid();
            UserId = userId;
            CreateAt = DateTimeOffset.UtcNow;
            UpdateAt = DateTimeOffset.UtcNow;
        }

        public void UpdateCart()
        {
            UpdateAt = DateTimeOffset.UtcNow;
        }
    }
}