using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Users.Commands.RegisterUser;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Users.Commands.RegisterUser;

[Trait("Layer", "Application")]
public sealed class RegisterUserCommandHandlerTests
{
    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidRepository()
    {
        // Act and Assert
        var exception = Record.Exception(() => 
            new RegisterUserCommandHandler
            (
                Mock.Of<IUserRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => 
            new RegisterUserCommandHandler
            (
                default!, // Force non-nullable UserRepository for testing
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenPasswordHasherIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => 
            new RegisterUserCommandHandler
            (
                Mock.Of<IUserRepository>(),
                default!, // Force non-nullable PasswordHasher for testing
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => 
            new RegisterUserCommandHandler
            (
                Mock.Of<IUserRepository>(),
                Mock.Of<IPasswordHasher>(),
                default! // Force non-nullable DateTimeProvider for testing
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldRegisterUser_WhenParametersAreValid()
    {
        // Arrange
        var userCommand = new RegisterUserCommand(
            "Briangel",
            "Santana Calcanio",
            "+18298881212",
            new DateOnly(2001, 1, 1),
            'M',
            "TestPassword123",
            "test@example.com"
        );

        var timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), CancellationToken.None)).ReturnsAsync(false);
        
        mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns(new String('a', 64));
        
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(timestamp);

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object
        );

        // Act
        var userDto = await handler.Handle(userCommand, CancellationToken.None);

        // Assert
        mockUserRepository.Verify(
            x => x.RegisterAsync(
                It.Is<User>(u =>
                    u.FirstName == userCommand.FirstName &&
                    u.LastName == userCommand.LastName &&
                    u.EmailAddress.Value == userCommand.Email &&
                    u.PhoneNumber!.Value == userCommand.PhoneNumber &&
                    u.BirthDate == userCommand.BirthDate &&
                    u.Gender == userCommand.Gender &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.CreatedAt == timestamp &&
                    u.UpdatedAt == timestamp &&
                    u.PasswordHash.Value == new string('a', 64)
                ),
                CancellationToken.None)
            , Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);

        Assert.NotNull(userDto);
        Assert.NotEqual(default, userDto.Id);
        Assert.Equal(userCommand.FirstName, userDto.FirstName);
        Assert.Equal(userCommand.LastName, userDto.LastName);
        Assert.Equal(userCommand.PhoneNumber, userDto.PhoneNumber);
        Assert.Equal(userCommand.BirthDate, userDto.BirthDate);
        Assert.Equal(userCommand.Gender, userDto.Gender);
        Assert.Equal(userCommand.Email, userDto.EmailAddress);
        Assert.True(userDto.IsActive);
        Assert.Equal(timestamp, userDto.CreatedAt);
        Assert.Equal(timestamp, userDto.UpdatedAt);
        Assert.False(userDto.IsDeleted);
    }
}