using Application.Auth.Commands.ChangePassword;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Common.Tests.Factories;
using Domain.Entities;
using MediatR;
using Moq;

namespace Application.Tests.Auth.Commands.ChangePassword;

[Trait("Layer", "Application")]
public sealed class ChangePasswordCommandHandlerTests
{
    private static ChangePasswordCommand CreateCommand(
        Ulid? userId = null,
        string currentPassword = "CurrentPassword123",
        string newPassword = "NewPassword123")
    {
        return new ChangePasswordCommand(userId ?? Ulid.NewUlid(), currentPassword, newPassword);
    }

    private static ChangePasswordCommandHandler CreateHandler(
        Mock<IRefreshTokenRepository> refreshTokenRepository,
        Mock<IUnitOfWork> unitOfWork,
        Mock<IUserRepository> userRepository,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<IPasswordHasher> passwordHasher)
    {
        return new ChangePasswordCommandHandler(
            refreshTokenRepository.Object,
            unitOfWork.Object,
            userRepository.Object,
            dateTimeProvider.Object,
            passwordHasher.Object
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new ChangePasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRefreshTokenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ChangePasswordCommandHandler(
                default!,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ChangePasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                default!,
                Mock.Of<IUserRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ChangePasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ChangePasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenPasswordHasherIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ChangePasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IDateTimeProvider>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Handle")]
    public async Task Handle_ShouldChangePasswordAndRevokeAllRefreshTokens_WhenRequestIsValid()
    {
        var user = UserTestFactory.CreateUser();
        var originalPasswordHash = user.PasswordHash.Value;
        var now = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var newHashedPassword = new string('b', 64);
        var command = CreateCommand(user.Id, "CurrentPassword123", "NewPassword123");

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(user);
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(now);
        mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.CurrentPassword, originalPasswordHash))
            .Returns(true);
        mockPasswordHasher
            .Setup(x => x.HashPassword(command.NewPassword))
            .Returns(newHashedPassword);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.Equal(newHashedPassword, user.PasswordHash.Value);
        Assert.Equal(now, user.UpdatedAt);

        mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, CancellationToken.None), Times.Once);
        mockPasswordHasher.Verify(x => x.VerifyPassword(command.CurrentPassword, originalPasswordHash), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(command.NewPassword), Times.Once);
        mockUserRepository.Verify(x => x.UpdateAsync(user, CancellationToken.None), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(user.Id, now, CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var user = UserTestFactory.CreateUser();
        var now = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var newHashedPassword = new string('b', 64);
        var command = CreateCommand(user.Id, "CurrentPassword123", "NewPassword123");

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByIdAsync(user.Id, token)).ReturnsAsync(user);
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(now);
        mockPasswordHasher.Setup(x => x.VerifyPassword(command.CurrentPassword, user.PasswordHash.Value)).Returns(true);
        mockPasswordHasher.Setup(x => x.HashPassword(command.NewPassword)).Returns(newHashedPassword);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, token), Times.Once);
        mockUserRepository.Verify(x => x.UpdateAsync(user, token), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(user.Id, now, token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        var command = CreateCommand();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(It.IsAny<Ulid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
    {
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero));
        var command = CreateCommand(user.Id, "CurrentPassword123", "NewPassword123");

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(It.IsAny<Ulid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenCurrentPasswordIsInvalid()
    {
        var user = UserTestFactory.CreateUser();
        var command = CreateCommand(user.Id, "WrongPassword123", "NewPassword123");

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.CurrentPassword, user.PasswordHash.Value))
            .Returns(false);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidCredentialsException>(() => handler.Handle(command, CancellationToken.None));

        mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(It.IsAny<Ulid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/ChangePasswordCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenHashPasswordFails()
    {
        var user = UserTestFactory.CreateUser();
        var command = CreateCommand(user.Id, "CurrentPassword123", "NewPassword123");

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockPasswordHasher
            .Setup(x => x.VerifyPassword(command.CurrentPassword, user.PasswordHash.Value))
            .Returns(true);
        mockPasswordHasher
            .Setup(x => x.HashPassword(command.NewPassword))
            .Throws(new ArgumentException("Password hashing failed."));

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(It.IsAny<Ulid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}