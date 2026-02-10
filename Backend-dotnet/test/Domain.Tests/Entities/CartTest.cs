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
            var utcNow = DateTimeOffset.UtcNow;

            // Act
            var cart = new Cart(id, userId, utcNow);

            // Assert
            Assert.NotNull(cart);
            Assert.Equal(id, cart.Id);
            Assert.Equal(userId, cart.UserId);
            Assert.NotEqual(Ulid.Empty, cart.Id);
            Assert.Equal(utcNow, cart.CreatedAt);
            Assert.Equal(utcNow, cart.UpdatedAt);
            Assert.True(cart.CreatedAt <= DateTimeOffset.UtcNow);
            Assert.True(cart.UpdatedAt <= DateTimeOffset.UtcNow);
        }

        [Fact]
        public void CreateCart_ShouldThrowException_WhenIdIsEmpty(){
            // Arrange
            var id = Ulid.Empty;
            var userId = Ulid.NewUlid();
            var utcNow = DateTimeOffset.UtcNow;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cart(id, userId, utcNow));
            Assert.Contains("Id is required.", exception.Message);
        }

        [Fact]
        public void CreateCart_ShouldThrowException_WhenUserIdIsEmpty()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.Empty;
            var utcNow = DateTimeOffset.UtcNow;

            // Act and Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cart(id, userId, utcNow));
            Assert.Contains("User Id is required.", exception.Message);
        }

        [Fact]
        public void UpdateCart_ShouldUpdatePropertyUpdateAt()
        {
            // Arrange
            var id = Ulid.NewUlid();
            var userId = Ulid.NewUlid();
            var utcNow = DateTimeOffset.UtcNow;
            var cart = new Cart(id, userId, utcNow);
            var originalUpdateAt = cart.UpdatedAt;

            // Act
            cart.UpdateCart(utcNow.AddMinutes(1));
            // Assert
            Assert.True(cart.UpdatedAt > originalUpdateAt);
        }
    }
}