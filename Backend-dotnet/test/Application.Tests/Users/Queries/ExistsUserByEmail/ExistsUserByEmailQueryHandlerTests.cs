using Application.Users.Queries.ExistsUserByEmail;
using Application.Common.Abstractions.Persistence;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Users.Queries.ExistsUserByEmail;

[Trait("Layer", "Application")]
public sealed class ExistsUserByEmailQueryHandlerTests
{
    [Fact]
    [Trait("Users", "Queries/ExistsUserByEmail/Constructor")]
    public void Constructor_ShouldInitialize_WithValidRepository()
    {
        // Act and Assert
        var exception = Record.Exception(() => new ExistsUserByEmailQueryHandler(Mock.Of<IUserRepository>()));

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Queries/ExistsUserByEmail/Constructor")]
    public void Constructor_ShouldThrowException_WhenRepositoryIsNull()
    {
        // Arrange
        Assert.Throws<ArgumentNullException>(() => new ExistsUserByEmailQueryHandler(default!)); // Force non-nullable for testing
    }

    [Fact]
    [Trait("Users", "Queries/ExistsUserByEmail/Handle")]
    public async Task Handle_ShouldReturnTrue_WhenUserExists()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        var mockUserRepository = new Mock<IUserRepository>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new ExistsUserByEmailQueryHandler(mockUserRepository.Object);

        // Act
        var result = await handler.Handle(new ExistsUserByEmailQuery(email.Value), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(email, CancellationToken.None), Times.Once);

        Assert.True(result);
    }

    [Fact]
    [Trait("Users", "Queries/ExistsUserByEmail/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        // Arrange
        var email = Email.Create("test@example.com");

        var mockUserRepository = new Mock<IUserRepository>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ExistsUserByEmailQueryHandler(mockUserRepository.Object);

        // Act
        var result = await handler.Handle(new ExistsUserByEmailQuery(email.Value), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(email, CancellationToken.None), Times.Once);
        Assert.False(result);
    }
}