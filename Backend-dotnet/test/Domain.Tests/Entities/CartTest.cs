using Domain.Entities;

namespace Domain.Test.Entities
{
    public class CartTest
    {
        [Fact]
        public void CreateCart_ShouldCreateCartWithValidUserId()
        {
            // Arrange
            var userId = Ulid.NewUlid();

            // Act
            var cart = new Cart(userId);

            // Assert
            Assert.NotNull(cart);
            Assert.Equal(userId, cart.UserId);
            Assert.NotEqual(Ulid.Empty, cart.Id);
            Assert.True(cart.CreateAt <= DateTimeOffset.UtcNow);
            Assert.True(cart.UpdateAt <= DateTimeOffset.UtcNow);
        }

        [Fact]
        public void CreateCart_ShouldThrowException_WhenUserIdIsEmpty()
        {
            // Arrange
            var userId = Ulid.Empty;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cart(userId));
            Assert.Equal("User Id is required. (Parameter 'userId')", exception.Message);
        }

        [Fact]
        public void UpdateCart_ShouldUpdatePropertyUpdateAt()
        {
            // Arrange
            var userId = Ulid.NewUlid();
            var cart = new Cart(userId);
            var originalUpdateAt = cart.UpdateAt;

            // Act
            cart.UpdateCart();

            // Assert
            Assert.True(cart.UpdateAt > originalUpdateAt);
        }
    }
}