using Application.Users.Queries.GetUserByEmail;
using Application.Common.Abstractions.Persistence;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Users.Queries.GetUserByEmail;

[Trait("Layer", "Application")]
public sealed class GetUserByEmailQueryHandlerTests
{
    [Fact]
    [Trait("Users", "GetUserByEmailQueryHandler")]
    public void Constructur_ShouldInitialize_WithValidRepository()
    {
        // Act and Assert
        var exception = Record.Exception(() => new GetUserByEmailQueryHandler(Mock.Of<IUserRepository>()));

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "GetUserByEmailQueryHandler")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => new GetUserByEmailQueryHandler(default!)); // Force non-nullable for testing
    }

    [Fact]
    [Trait("Users", "GetUserByEmailQueryHandler")]
    public async Task Handler_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = User.Create(
            Ulid.NewUlid(),
            "Briangel",
            "Santana Calcanio",
            new DateOnly(2001, 1, 1),
            'M',
            PasswordHash.From(new String('a', 64)),
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Email.Create("test@example.com"),
            PhoneNumber.Create("+18298881212")
        );

        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserByEmailQueryHandler(mockUserRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByEmailQuery(user.EmailAddress.Value), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByEmailAsync(user.EmailAddress, CancellationToken.None), Times.Once);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.BirthDate, result.BirthDate);
        Assert.Equal(user.Gender, result.Gender);
        Assert.Equal(user.IsActive, result.IsActive);
        Assert.Equal(user.CreatedAt, result.CreatedAt);
        Assert.Equal(user.UpdatedAt, result.UpdatedAt);
        Assert.Equal(user.EmailAddress.Value, result.EmailAddress);
        Assert.Equal(user.IsDeleted, result.IsDeleted);
        Assert.Equal(user.PhoneNumber?.Value, result.PhoneNumber);
    }

    [Fact]
    [Trait("Users", "GetUserByEmailQueryHandler")]
    public async Task Handler_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByEmailQueryHandler(mockUserRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByEmailQuery("test@example.com"), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Null(result);
    }
}