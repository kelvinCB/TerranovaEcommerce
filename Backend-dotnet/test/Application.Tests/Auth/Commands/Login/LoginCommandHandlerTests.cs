using Application.Auth.Commands.Login;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Auth;
using Application.Common.Exceptions;
using Common.Tests.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Auth.Commands.Login;

[Trait("Layer", "Application")]
public sealed class LoginCommandHandlerTests
{
    private static LoginCommand CreateCommand(
        string emailAddress = "test@example.com",
        string password = "TestPassword123")
    {
        return new LoginCommand(emailAddress, password);
    }

    private static LoginCommandHandler CreateHandler(
        Mock<IUserRepository> userRepository,
        Mock<IUserRoleRepository> userRoleRepository,
        Mock<IRefreshTokenRepository> refreshTokenRepository,
        Mock<IUnitOfWork> unitOfWork,
        Mock<IAuthSessionService> authSessionService,
        Mock<IPasswordHasher> passwordHasher)
    {
        return new LoginCommandHandler(
            userRepository.Object,
            userRoleRepository.Object,
            refreshTokenRepository.Object,
            unitOfWork.Object,
            authSessionService.Object,
            passwordHasher.Object
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new LoginCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IPasswordHasher>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoginCommandHandler(
                default!,
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoginCommandHandler(
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRefreshTokenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoginCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                default!,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoginCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IRefreshTokenRepository>(),
                default!,
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenAuthSessionServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoginCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenPasswordHasherIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LoginCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IAuthSessionService>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Handle")]
    public async Task Handle_ShouldReturnAuthSession_WhenCredentialsAreValid()
    {
        var user = UserTestFactory.CreateUser();
        var roles = new[] { RoleTestFactory.CreateRole() };
        var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
        var session = new AuthSessionResult
        {
            AccessToken = "access-token",
            AccessTokenExpiresAt = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero),
            RefreshToken = "refresh-token",
            RefreshTokenEntity = refreshToken
        };
        var command = CreateCommand(user.EmailAddress.Value, "TestPassword123");

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash.Value))
            .Returns(true);
        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        mockAuthSessionService
            .Setup(x => x.Create(user, It.Is<IReadOnlyCollection<Role>>(r => r.Count == roles.Length)))
            .Returns(session);

        var handler = CreateHandler(
            mockUserRepository,
            mockUserRoleRepository,
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockAuthSessionService,
            mockPasswordHasher);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(session.AccessToken, result.AccessToken);
        Assert.Equal(session.RefreshToken, result.RefreshToken);
        Assert.Equal(session.AccessTokenExpiresAt, result.AccessTokenExpiresAt);
        Assert.Equal(session.RefreshTokenEntity.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.FirstName, result.User.FirstName);
        Assert.Equal(user.LastName, result.User.LastName);
        Assert.Equal(user.EmailAddress.Value, result.User.Email);
        Assert.Single(result.User.Roles);
        Assert.Equal(roles[0].Id, result.User.Roles.First().Id);
        Assert.Equal(roles[0].Name, result.User.Roles.First().Name);
        Assert.Equal(roles[0].Description, result.User.Roles.First().Description);

        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), CancellationToken.None), Times.Once);
        mockPasswordHasher.Verify(x => x.VerifyPassword(command.Password, user.PasswordHash.Value), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(user.Id, CancellationToken.None), Times.Once);
        mockAuthSessionService.Verify(x => x.Create(user, It.Is<IReadOnlyCollection<Role>>(r => r.Count == roles.Length)), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(session.RefreshTokenEntity, CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var user = UserTestFactory.CreateUser();
        var roles = new[] { RoleTestFactory.CreateRole() };
        var session = new AuthSessionResult
        {
            AccessToken = "access-token",
            AccessTokenExpiresAt = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero),
            RefreshToken = "refresh-token",
            RefreshTokenEntity = RefreshTokenTestFactory.CreateRefreshToken()
        };
        var command = CreateCommand(user.EmailAddress.Value, "TestPassword123");

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), token)).ReturnsAsync(user);
        mockPasswordHasher.Setup(x => x.VerifyPassword(command.Password, user.PasswordHash.Value)).Returns(true);
        mockUserRoleRepository.Setup(x => x.GetByUserIdAsync(user.Id, token)).ReturnsAsync(roles);
        mockAuthSessionService.Setup(x => x.Create(user, It.IsAny<IReadOnlyCollection<Role>>())).Returns(session);

        var handler = CreateHandler(
            mockUserRepository,
            mockUserRoleRepository,
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockAuthSessionService,
            mockPasswordHasher);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByEmailAsync(It.IsAny<Email>(), token), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(user.Id, token), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(session.RefreshTokenEntity, token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        var command = CreateCommand();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(
            mockUserRepository,
            mockUserRoleRepository,
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockAuthSessionService,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockAuthSessionService.Verify(x => x.Create(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<Role>>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
    {
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero));
        var command = CreateCommand(user.EmailAddress.Value, "TestPassword123");

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(
            mockUserRepository,
            mockUserRoleRepository,
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockAuthSessionService,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockAuthSessionService.Verify(x => x.Create(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<Role>>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenPasswordIsInvalid()
    {
        var user = UserTestFactory.CreateUser();
        var command = CreateCommand(user.EmailAddress.Value, "WrongPassword123");

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash.Value))
            .Returns(false);

        var handler = CreateHandler(
            mockUserRepository,
            mockUserRoleRepository,
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockAuthSessionService,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockAuthSessionService.Verify(x => x.Create(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<Role>>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/LoginCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserHasNoRoles()
    {
        var user = UserTestFactory.CreateUser();
        var command = CreateCommand(user.EmailAddress.Value, "TestPassword123");

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.Password, user.PasswordHash.Value))
            .Returns(true);
        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Role>());

        var handler = CreateHandler(
            mockUserRepository,
            mockUserRoleRepository,
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockAuthSessionService,
            mockPasswordHasher);

        await Assert.ThrowsAsync<AtLeastOneRoleMustBeProvidedException>(() => handler.Handle(command, CancellationToken.None));

        mockAuthSessionService.Verify(x => x.Create(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<Role>>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}