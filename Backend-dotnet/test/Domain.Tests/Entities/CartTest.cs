using Domain.Entities;
using Domain.Tests.Factories;

namespace Domain.Tests.Entities;

[Trait("Layer", "Domain")]
public class CartTest
{
    [Fact]
    [Trait("Cart", "Create")]
    public void Create_ShouldCreateCart_WhenParametersAreValid()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var timestamp = DateTimeOffset.UtcNow;

        // Act
        var cart = Cart.Create(id, userId, timestamp);

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
    [Trait("Cart", "Create")]
    public void Create_ShouldThrowException_WhenIdIsEmpty()
    {
        // Arrange
        var id = Ulid.Empty;
        var userId = Ulid.NewUlid();
        var timestamp = DateTimeOffset.UtcNow;

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Cart.Create(id, userId, timestamp));

        Assert.Contains("Is Uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Cart", "Create")]
    public void Create_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.Empty;
        var timestamp = DateTimeOffset.UtcNow;

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Cart.Create(id, userId, timestamp));

        Assert.Contains("Is Uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Cart", "Create")]
    public void Create_ShouldThrowException_WhenCreatedAtIsNotUtc()
    {
        // Arrange
        var id = Ulid.NewUlid();
        var userId = Ulid.NewUlid();
        var timestamp = DateTimeOffset.Now;

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => Cart.Create(id, userId, timestamp));

        Assert.Contains("Timestamp must be in UTC (offset 00:00).", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Cart", "Update")]
    public void Update_ShouldUpdatePropertyUpdatedAt()
    {
        // Arrange
        var cart = CartTestFactory.CreateCart();
        var originalUpdatedAt = cart.UpdatedAt;

        // Act
        cart.Update(cart.UpdatedAt.AddMinutes(1));

        // Assert
        Assert.True(cart.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    [Trait("Cart", "Update")]
    public void Update_ShouldThrowException_WhenUpdatedAtIsBeforeCreatedAt()
    {
        // Arrange
        var cart = CartTestFactory.CreateCart();

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => cart.Update(cart.UpdatedAt.AddMinutes(-1)));

        Assert.Contains("Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Cart", "Update")]
    public void Update_ShouldThrowException_WhenUpdatedAtIsDefault()
    {
        // Arrange
        var cart = CartTestFactory.CreateCart();

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => cart.Update(default));

        Assert.Contains("Timestamp is uninitialized.", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("Cart", "Update")]
    public void Update_ShouldThrowException_WhenUpdatedAtIsNotUtc()
    {
        // Arrange
        var cart = CartTestFactory.CreateCart();
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => cart.Update(timestamp));

        Assert.Contains("Timestamp must be in UTC (offset 00:00).", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
}
