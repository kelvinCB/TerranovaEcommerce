using Domain.Entities;

namespace Domain.Tests.Entities;

[Trait("Layer", "Domain")]
public class UserRoleTest
{
    [Fact]
    [Trait("UserRoles", "Create")]
    public void Create_ShouldCreateUserRole_WhenParametersAreValid()
    {
        // Arrange
        var userId = Ulid.NewUlid();
        var roleId = Ulid.NewUlid();
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        var userRole = UserRole.Create(userId, roleId, timestamp);

        // Assert
        Assert.NotNull(userRole);
        Assert.Equal(userId, userRole.UserId);
        Assert.Equal(roleId, userRole.RoleId);
        Assert.Equal(timestamp, userRole.CreatedAt);
    }

    [Fact]
    [Trait("UserRoles", "Create")]
    public void Create_ShouldThrowException_WhenUserIdIsEmpty()
    {
        // Arrange
        var userId = Ulid.Empty;
        var roleId = Ulid.NewUlid();
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => UserRole.Create(userId, roleId, timestamp));

        Assert.Contains("Is Uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserRoles", "Create")]
    public void Create_ShouldThrowException_WhenRoleIdIsEmpty()
    {
        // Arrange
        var userId = Ulid.NewUlid();
        var roleId = Ulid.Empty;
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => UserRole.Create(userId, roleId, timestamp));

        Assert.Contains("Is Uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserRoles", "Create")]
    public void Create_ShouldThrowException_WhenTimestampIsUninitialized()
    {
        // Arrange
        var userId = Ulid.NewUlid();
        var roleId = Ulid.NewUlid();
        var timestamp = default(DateTimeOffset);

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => UserRole.Create(userId, roleId, timestamp));

        Assert.Contains("Is Uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    [Trait("UserRoles", "Create")]
    public void Create_ShouldThrowException_WhenTimestampIsNotUtc()
    {
        // Arrange
        var userId = Ulid.NewUlid();
        var roleId = Ulid.NewUlid();
        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4));

        // Act and Assert
        var exception = Assert.Throws<ArgumentException>(() => UserRole.Create(userId, roleId, timestamp));

        Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
    }
    
}