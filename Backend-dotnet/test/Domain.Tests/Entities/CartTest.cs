using Domain.Entities;

namespace Domain.Test.Entities
{
    public class CartTest
    {
        [Fact]
        public void CreateCart_ShouldCreateCartWithValidUserId()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.UtcNow;

            // Act
            var cart = new Cart(id, userId, timestamp);

            // Assert
            Assert.NotNull(cart);
            Assert.Equal(id, cart.Id);
            Assert.Equal(userId, cart.UserId);
            Assert.NotEqual(Ulid.Empty, cart.Id);
            Assert.Equal(timestamp, cart.CreatedAt);
            Assert.Equal(timestamp, cart.UpdatedAt);
            Assert.True(cart.CreatedAt <= DateTimeOffset.UtcNow);
            Assert.True(cart.UpdatedAt <= DateTimeOffset.UtcNow);
        }

        [Fact]
        public void CreateCart_ShouldThrowException_WhenIdIsEmpty(){
            // Arrange
            var id = Ulid.Empty;
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.UtcNow;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cart(id, userId, timestamp));
            Assert.Contains("Id is required.", exception.Message);
        }

        [Fact]
        public void CreateCart_ShouldThrowException_WhenUserIdIsEmpty()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.Empty;
            var timestamp = DateTimeOffset.UtcNow;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cart(id, userId, timestamp));
            Assert.Contains("User Id is required.", exception.Message);
        }

        [Fact]
        public void CreateCart_ShouldThrowException_WhenCreatedAtIsNotUtc()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.Now;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cart(id, userId, timestamp));
            Assert.Contains("Timestamp must be in UTC (offset 00:00).", exception.Message);
        }

        [Fact]
        public void UpdateCart_ShouldUpdatePropertyUpdatedAt()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.UtcNow;
            var cart = new Cart(id, userId, timestamp);
            var originalUpdatedAt = cart.UpdatedAt;

            // Act
            cart.UpdateCart(timestamp.AddMinutes(1));
            // Assert
            Assert.True(cart.UpdatedAt > originalUpdatedAt);
        }

        [Fact]
        public void UpdateCart_ShouldThrowException_WhenUpdatedAtIsBeforeCreatedAt()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.UtcNow;
            var cart = new Cart(id, userId, timestamp);

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => cart.UpdateCart(timestamp.AddMinutes(-1)));
            Assert.Contains("UpdatedAt cannot be before CreatedAt.", exception.Message);
        }

        [Fact]
        public void UpdateCart_ShouldThrowException_WhenUpdatedAtIsDefault()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.UtcNow;
            var cart = new Cart(id, userId, timestamp);

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => cart.UpdateCart(default));
            Assert.Contains("Timestamp is uninitialized.", exception.Message);
        }

        [Fact]
        public void UpdateCart_ShouldThrowException_WhenUpdatedAtIsNotUtc()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var timestamp = DateTimeOffset.UtcNow;
            var cart = new Cart(id, userId, timestamp);

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => cart.UpdateCart(DateTimeOffset.Now));
            Assert.Contains("Timestamp must be in UTC (offset 00:00).", exception.Message);
        }
    }
}