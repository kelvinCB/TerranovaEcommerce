using Application.Auth.Commands.RefreshSession;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Auth;
using Application.Common.Exceptions;
using Common.Tests.Factories;
using Domain.Entities;
using Moq;
using RefreshTokenEntity = Domain.Entities.RefreshToken;

namespace Application.Tests.Auth.Commands.RefreshSession;

[Trait("Layer", "Application")]
public sealed class RefreshSessionCommandHandlerTests
{
    private static RefreshSessionCommand CreateCommand(string refreshToken = "plain-refresh-token")
    {
        return new RefreshSessionCommand(refreshToken);
    }

    private static RefreshSessionCommandHandler CreateHandler(
        Mock<IRefreshTokenRepository> refreshTokenRepository,
        Mock<IUnitOfWork> unitOfWork,
        Mock<IUserRepository> userRepository,
        Mock<IUserRoleRepository> userRoleRepository,
        Mock<IAuthSessionService> authSessionService,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<ITokenHashService> tokenHashService)
    {
        return new RefreshSessionCommandHandler(
            refreshTokenRepository.Object,
            unitOfWork.Object,
            userRepository.Object,
            userRoleRepository.Object,
            authSessionService.Object,
            dateTimeProvider.Object,
            tokenHashService.Object
        );
    }

    private static User CreateUser(Ulid? id = null)
    {
        return User.Create(
            id ?? Ulid.NewUlid(),
            "David",
            "Calcanio Hernandez",
            new DateOnly(2001, 1, 1),
            'M',
            Domain.ValueObjects.PasswordHash.From(new string('a', 64)),
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            Domain.ValueObjects.Email.Create("test@example.com"),
            Domain.ValueObjects.PhoneNumber.Create("+18298881212")
        );
    }

    private static RefreshTokenEntity CreateRefreshToken(Ulid userId, string tokenHash = "hashed-refresh-token")
    {
        return RefreshTokenEntity.Create(
            id: Ulid.NewUlid(),
            userId: userId,
            tokenHash: tokenHash,
            jwtId: "jwt-id-001",
            expiresAt: new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero),
            createdAt: new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
            userAgent: "user-agent",
            ipAddress: "127.0.0.1"
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRefreshTokenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                default!,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                default!,
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenAuthSessionServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IAuthSessionService>(),
                default!,
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenTokenHashServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RefreshSessionCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IAuthSessionService>(),
                Mock.Of<IDateTimeProvider>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldReturnAuthSession_WhenRefreshTokenIsValid()
    {
        var now = new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var user = CreateUser();
        var existingRefreshToken = CreateRefreshToken(user.Id, hashedToken);
        var roles = new[] { RoleTestFactory.CreateRole() };
        var rotatedRefreshToken = RefreshTokenTestFactory.CreateRefreshToken();
        var session = new AuthSessionResult
        {
            AccessToken = "access-token",
            AccessTokenExpiresAt = new DateTimeOffset(2026, 1, 6, 0, 0, 0, TimeSpan.Zero),
            RefreshToken = "new-refresh-token",
            RefreshTokenEntity = rotatedRefreshToken
        };

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(now);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);
        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);
        mockAuthSessionService
            .Setup(x => x.Create(user, It.Is<IReadOnlyCollection<Role>>(r => r.Count == roles.Length)))
            .Returns(session);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(session.AccessToken, result.AccessToken);
        Assert.Equal(session.AccessTokenExpiresAt, result.AccessTokenExpiresAt);
        Assert.Equal(session.RefreshToken, result.RefreshToken);
        Assert.Equal(session.RefreshTokenEntity.ExpiresAt, result.RefreshTokenExpiresAt);
        Assert.Equal(user.Id, result.User.Id);
        Assert.Equal(user.FirstName, result.User.FirstName);
        Assert.Equal(user.LastName, result.User.LastName);
        Assert.Equal(user.EmailAddress.Value, result.User.Email);
        Assert.Single(result.User.Roles);
        Assert.Equal(roles[0].Id, result.User.Roles.First().Id);
        Assert.Equal(roles[0].Name, result.User.Roles.First().Name);
        Assert.Equal(roles[0].Description, result.User.Roles.First().Description);

        Assert.True(existingRefreshToken.IsRevoked);
        Assert.Equal(now, existingRefreshToken.RevokedAt);
        Assert.Equal(session.RefreshTokenEntity.Id, existingRefreshToken.ReplacedByTokenId);

        mockTokenHashService.Verify(x => x.HashToken(command.RefreshToken), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.GetByTokenHashAsync(hashedToken, CancellationToken.None), Times.Once);
        mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, CancellationToken.None), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(user.Id, CancellationToken.None), Times.Once);
        mockAuthSessionService.Verify(x => x.Create(user, It.Is<IReadOnlyCollection<Role>>(r => r.Count == roles.Length)), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(existingRefreshToken, CancellationToken.None), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(session.RefreshTokenEntity, CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var now = new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var user = CreateUser();
        var existingRefreshToken = CreateRefreshToken(user.Id, hashedToken);
        var roles = new[] { RoleTestFactory.CreateRole() };
        var session = new AuthSessionResult
        {
            AccessToken = "access-token",
            AccessTokenExpiresAt = new DateTimeOffset(2026, 1, 6, 0, 0, 0, TimeSpan.Zero),
            RefreshToken = "new-refresh-token",
            RefreshTokenEntity = RefreshTokenTestFactory.CreateRefreshToken()
        };

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(now);
        mockRefreshTokenRepository.Setup(x => x.GetByTokenHashAsync(hashedToken, token)).ReturnsAsync(existingRefreshToken);
        mockUserRepository.Setup(x => x.GetByIdAsync(user.Id, token)).ReturnsAsync(user);
        mockUserRoleRepository.Setup(x => x.GetByUserIdAsync(user.Id, token)).ReturnsAsync(roles);
        mockAuthSessionService.Setup(x => x.Create(user, It.IsAny<IReadOnlyCollection<Role>>())).Returns(session);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        await handler.Handle(command, token);

        mockRefreshTokenRepository.Verify(x => x.GetByTokenHashAsync(hashedToken, token), Times.Once);
        mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, token), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(user.Id, token), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(existingRefreshToken, token), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(session.RefreshTokenEntity, token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenRefreshTokenDoesNotExist()
    {
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync((RefreshTokenEntity?)null);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenRefreshTokenIsNotActive()
    {
        var now = new DateTimeOffset(2026, 1, 11, 0, 0, 0, TimeSpan.Zero);
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var user = CreateUser();
        var existingRefreshToken = CreateRefreshToken(user.Id, hashedToken);

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(now);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        await Assert.ThrowsAsync<RefreshTokenNotActiveException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        var now = new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var userId = Ulid.NewUlid();
        var existingRefreshToken = CreateRefreshToken(userId, hashedToken);

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(now);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);
        mockUserRepository
            .Setup(x => x.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
    {
        var now = new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var user = CreateUser();
        user.SetIsDeleted(true, now);
        var existingRefreshToken = CreateRefreshToken(user.Id, hashedToken);

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(now);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);
        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RefreshSessionCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserHasNoRoles()
    {
        var now = new DateTimeOffset(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);
        var command = CreateCommand();
        var hashedToken = "hashed-refresh-token";
        var user = CreateUser();
        var existingRefreshToken = CreateRefreshToken(user.Id, hashedToken);

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockAuthSessionService = new Mock<IAuthSessionService>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(hashedToken);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(now);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(hashedToken, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRefreshToken);
        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<Role>());

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserRoleRepository,
            mockAuthSessionService,
            mockDateTimeProvider,
            mockTokenHashService);

        await Assert.ThrowsAsync<UserMustHaveAtLeastOneRoleException>(() => handler.Handle(command, CancellationToken.None));

        mockAuthSessionService.Verify(x => x.Create(It.IsAny<User>(), It.IsAny<IReadOnlyCollection<Role>>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.AddAsync(It.IsAny<RefreshTokenEntity>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
