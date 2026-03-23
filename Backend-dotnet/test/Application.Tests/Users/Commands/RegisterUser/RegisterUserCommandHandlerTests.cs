using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Users.Commands.RegisterUser;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Users.Commands.RegisterUser;

[Trait("Layer", "Application")]
public sealed class RegisterUserCommandHandlerTests
{
    private readonly DateTimeOffset _timestamp = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    private RegisterUserCommandHandler CreateHandler(
        Mock<IUserRepository> userRepository,
        Mock<IRoleRepository> roleRepository,
        Mock<IUserRoleRepository> userRoleRepository,
        Mock<IPasswordHasher> passwordHasher,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<IIdGenerator> idGenerator,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new RegisterUserCommandHandler(
            userRepository.Object,
            roleRepository.Object,
            userRoleRepository.Object,
            passwordHasher.Object,
            dateTimeProvider.Object,
            idGenerator.Object,
            unitOfWork.Object
        );
    }

    private static RegisterUserCommand CreateCommand(
        string? phoneNumber = "+18298881212",
        IReadOnlyCollection<Ulid>? roleIds = null)
    {
        return new RegisterUserCommand(
            "Briangel",
            "Santana Calcanio",
            phoneNumber,
            new DateOnly(2001, 1, 1),
            'M',
            "TestPassword123",
            "test@example.com",
            roleIds ?? new[] { Ulid.NewUlid() }
        );   
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        // Act and Assert
        var exception = Record.Exception(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IRoleRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IUnitOfWork>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                default!,
                Mock.Of<IRoleRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRoleRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IRoleRepository>(),
                default!,
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenPasswordHasherIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IRoleRepository>(),
                Mock.Of<IUserRoleRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IRoleRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IPasswordHasher>(),
                default!,
                Mock.Of<IIdGenerator>(),
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenIdGeneratorIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IRoleRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                default!,
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RegisterUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IRoleRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IPasswordHasher>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldRegisterUser_WhenParametersAreValid()
    {
        // Arrange
        var roleIds = new[] { Ulid.NewUlid() };
        var userCommand = CreateCommand(roleIds: roleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var userId = Ulid.NewUlid();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockRoleRepository
            .Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(roleIds);
        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Returns(new string('a', 64));
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(_timestamp);
        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Returns(userId);

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act
        var result = await handler.Handle(userCommand, CancellationToken.None);

        // Assert
        Assert.Equal(userId, result);
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), CancellationToken.None), Times.Once);
        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.Is<IReadOnlyCollection<Ulid>>(ids => roleIds.All(ids.Contains)),CancellationToken.None),Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(userCommand.Password), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
        mockUserRepository.Verify(x => x.RegisterAsync(
                It.Is<User>(u =>
                    u.Id == userId &&
                    u.FirstName == userCommand.FirstName &&
                    u.LastName == userCommand.LastName &&
                    u.EmailAddress.Value == userCommand.Email &&
                    u.PhoneNumber!.Value == userCommand.PhoneNumber &&
                    u.BirthDate == userCommand.BirthDate &&
                    u.Gender == userCommand.Gender &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.CreatedAt == _timestamp &&
                    u.UpdatedAt == _timestamp &&
                    u.PasswordHash.Value == new string('a', 64)),
                CancellationToken.None),
            Times.Once);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(
                It.Is<IReadOnlyCollection<UserRole>>(roles =>
                    roles.Count == roleIds.Length &&
                    roles.All(r => r.UserId == userId) &&
                    roles.Select(r => r.RoleId).OrderBy(x => x).SequenceEqual(roleIds.OrderBy(x => x)) &&
                    roles.All(r => r.CreatedAt == _timestamp)),
                CancellationToken.None),
            Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldPropagate_CancellationToken()
    {
        // Arrange
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var roleIds = new[] { Ulid.NewUlid() };
        var userCommand = CreateCommand(roleIds: roleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), token)).ReturnsAsync(false);
        mockRoleRepository.Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), token)).ReturnsAsync(roleIds);
        mockPasswordHasher.Setup(x => x.HashPassword(userCommand.Password)).Returns(new string('a', 64));
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(_timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(Ulid.NewUlid());

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act
        await handler.Handle(userCommand, token);

        // Assert
        mockUserRepository.Verify(x => x.ExistsByEmailAsync(It.IsAny<Email>(), token), Times.Once);
        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), token), Times.Once);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), token), Times.Once);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenNoRolesAreProvided()
    {
        // Arrange
        var userCommand = CreateCommand(roleIds: []);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<AtLeastOneRoleMustBeProvidedException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockUserRepository.Verify(x => x.ExistsByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var userCommand = CreateCommand();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<EmailAlreadyInUseException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockUserRepository.Verify(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()), Times.Once);
        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldRegisterUser_WhenPhoneNumberIsNotProvided()
    {
        // Arrange
        var roleIds = new[] { Ulid.NewUlid() };
        var userCommand = CreateCommand(phoneNumber: null, roleIds: roleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        mockRoleRepository.Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(roleIds);
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(new string('a', 64));
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(_timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(Ulid.NewUlid());

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act
        var userId = await handler.Handle(userCommand, CancellationToken.None);

        // Assert
        Assert.NotEqual(default, userId);
        mockUserRepository.Verify(x => x.RegisterAsync(
                It.Is<User>(u =>
                    u.FirstName == userCommand.FirstName &&
                    u.LastName == userCommand.LastName &&
                    u.EmailAddress.Value == userCommand.Email &&
                    u.PhoneNumber == null &&
                    u.BirthDate == userCommand.BirthDate &&
                    u.Gender == userCommand.Gender &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.CreatedAt == _timestamp &&
                    u.UpdatedAt == _timestamp &&
                    u.PasswordHash.Value == new string('a', 64)),
                It.IsAny<CancellationToken>()),
            Times.Once);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldRegisterUser_WhenPhoneNumberIsWhiteSpace()
    {
        // Arrange
        var roleIds = new[] { Ulid.NewUlid() };
        var userCommand = CreateCommand(phoneNumber: "", roleIds: roleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository.Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        mockRoleRepository.Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>())).ReturnsAsync(roleIds);
        mockPasswordHasher.Setup(x => x.HashPassword(It.IsAny<string>())).Returns(new string('a', 64));
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(_timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(Ulid.NewUlid());

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act
        var userId = await handler.Handle(userCommand, CancellationToken.None);

        // Assert
        Assert.NotEqual(default, userId);
        mockUserRepository.Verify(x => x.RegisterAsync(
                It.Is<User>(u =>
                    u.FirstName == userCommand.FirstName &&
                    u.LastName == userCommand.LastName &&
                    u.EmailAddress.Value == userCommand.Email &&
                    u.PhoneNumber == null &&
                    u.BirthDate == userCommand.BirthDate &&
                    u.Gender == userCommand.Gender &&
                    u.IsActive &&
                    !u.IsDeleted &&
                    u.CreatedAt == _timestamp &&
                    u.UpdatedAt == _timestamp &&
                    u.PasswordHash.Value == new string('a', 64)),
                It.IsAny<CancellationToken>()),
            Times.Once);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenPasswordIsInvalid()
    {
        // Arrange
        var userCommand = CreateCommand();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Returns(new string('a', 63));

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenPasswordHasherFails()
    {
        // Arrange
        var userCommand = CreateCommand();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Throws(new ArgumentException("Password hashing failed."));

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenIdGeneratorFails()
    {
        // Arrange
        var userCommand = CreateCommand();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Returns(new string('a', 64));
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(_timestamp);
        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Throws(new ArgumentException("Id generation failed."));

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RegisterUser/RegisterUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenRolesNotFound()
    {
        // Arrange
        var existingRoleIds = new[] { Ulid.NewUlid() };
        var requestedRoleIds = new[] { Ulid.NewUlid(), existingRoleIds[0] };
        var userCommand = CreateCommand(roleIds: requestedRoleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockRoleRepository = new Mock<IRoleRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(Email.Create(userCommand.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockPasswordHasher
            .Setup(x => x.HashPassword(userCommand.Password))
            .Returns(new string('a', 64));
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(_timestamp);
        mockIdGenerator
            .Setup(x => x.NewUlid())
            .Returns(Ulid.NewUlid());
        mockRoleRepository
            .Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRoleIds);

        var handler = CreateHandler(
            mockUserRepository,
            mockRoleRepository,
            mockUserRoleRepository,
            mockPasswordHasher,
            mockDateTimeProvider,
            mockIdGenerator,
            mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<RolesNotFoundException>(() => handler.Handle(userCommand, CancellationToken.None));

        mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUserRepository.Verify(x => x.RegisterAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
