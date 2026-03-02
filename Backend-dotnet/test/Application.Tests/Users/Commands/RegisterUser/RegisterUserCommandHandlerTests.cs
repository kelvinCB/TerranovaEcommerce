using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Users.Commands.RegisterUser;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Users.Commands.RegisterUser;

[Trait("Layer", "Application")]
public sealed class RegisterUserCommandHandlerTests
{
    private readonly DateTimeOffset timestamp = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        // Act and Assert
        var exception = Record.Exception(() => 
            new RegisterUserCommandHandler
            (
                Mock.Of<IUserRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>()
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
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>()
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
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>()
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
                default!, // Force non-nullable DateTimeProvider for testing
                Mock.Of<IIdGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenIdGeneratorIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => 
            new RegisterUserCommandHandler
            (
                Mock.Of<IUserRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                default! // Force non-nullable IdGenerator for testing
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

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        
        mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns(new String('a', 64));
        
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(timestamp);

        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Returns(Ulid.NewUlid());

        mockUserRepository.Setup(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );

        // Act
        var userDto = await handler.Handle(userCommand, CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), CancellationToken.None), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
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
                It.IsAny<CancellationToken>())
            , Times.Once);

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
    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldPropagate_CancellationToken()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;

        var userCommand = new RegisterUserCommand(
            "Briangel",
            "Santana Calcanio",
            "+18298881212",
            new DateOnly(2001, 1, 1),
            'M',
            "TestPassword123",
            "test@example.com"
        );

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), token)).ReturnsAsync(false);
        mockPasswordHasher.Setup(x => x.HashPassword(userCommand.Password)).Returns(new String('a', 64));
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(Ulid.NewUlid());
        mockUserRepository.Setup(x => x.RegisterAsync(It.IsAny<User>(), token));

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );

        // Act
        await handler.Handle(userCommand, token);

        // Assert
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(It.IsAny<Email>(), token), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), token), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserAlreadyExists()
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

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );
        
        // Act and Assert
        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Never);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldRegisterUser_WhenPhoneNumberIsNotProvided()
    {
        // Arrange
        var userCommand = new RegisterUserCommand(
            "Briangel",
            "Santana Calcanio",
            default, // Phone number not provided
            new DateOnly(2001, 1, 1),
            'M',
            "TestPassword123",
            "test@example.com"
        );

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        mockUserRepository.Setup(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));
        
        mockPasswordHasher
            .Setup(x => x.HashPassword(It.IsAny<string>()))
            .Returns(new String('a', 64));
        
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(timestamp);

        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Returns(Ulid.NewUlid());

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );

        // Act
        var userDto = await handler.Handle(userCommand, CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
        mockUserRepository.Verify(
            x => x.RegisterAsync(
                It.Is<User>(u =>
                    u.FirstName == userCommand.FirstName &&
                    u.LastName == userCommand.LastName &&
                    u.EmailAddress.Value == userCommand.Email &&
                    u.PhoneNumber == null &&
                    u.BirthDate == userCommand.BirthDate &&
                    u.Gender == userCommand.Gender &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.CreatedAt == timestamp &&
                    u.UpdatedAt == timestamp &&
                    u.PasswordHash.Value == new string('a', 64)
                ),
                It.IsAny<CancellationToken>())
            , Times.Once);

        Assert.NotNull(userDto);
        Assert.NotEqual(default, userDto.Id);
        Assert.Equal(userCommand.FirstName, userDto.FirstName);
        Assert.Equal(userCommand.LastName, userDto.LastName);
        Assert.Null(userDto.PhoneNumber);
        Assert.Equal(userCommand.BirthDate, userDto.BirthDate);
        Assert.Equal(userCommand.Gender, userDto.Gender);
        Assert.Equal(userCommand.Email, userDto.EmailAddress);
        Assert.True(userDto.IsActive);
        Assert.Equal(timestamp, userDto.CreatedAt);
        Assert.Equal(timestamp, userDto.UpdatedAt);
        Assert.False(userDto.IsDeleted);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenPasswordIsInvalid()
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

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Returns(new String('a', 63)); // Password too short
        
        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );
        
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenPasswordHasherFails()
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

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Throws(new ArgumentException("Password hashing failed."));

        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Returns(Ulid.NewUlid());

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );
        
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenIdGeneratorFails()
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

        var mockUserRepository = new Mock<IUserRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Returns(new String('a', 64));

        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Throws(new ArgumentException("Id generation failed."));

        var handler = new RegisterUserCommandHandler(
            mockUserRepository.Object,
            mockPasswordHasher.Object,
            mockDateTimeProvider.Object,
            mockIdGenerator.Object
        );
        
        // Act and Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

}