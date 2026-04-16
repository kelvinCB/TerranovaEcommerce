using Application.Auth.Commands.ResetPassword;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Common.Tests.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Moq;

namespace Application.Tests.Auth.Commands.ResetPassword;

[Trait("Layer", "Application")]
public sealed class ResetPasswordCommandHandlerTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static ResetPasswordCommand CreateCommand(
        string email = "test@example.com",
        string code = "123456",
        string newPassword = "NewPassword123")
    {
        return new ResetPasswordCommand(email, code, newPassword);
    }

    private static ResetPasswordCommandHandler CreateHandler(
        Mock<IRefreshTokenRepository> refreshTokenRepository,
        Mock<IUnitOfWork> unitOfWork,
        Mock<IUserRepository> userRepository,
        Mock<IUserVerificationRepository> userVerificationRepository,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<IPasswordHasher> passwordHasher)
    {
        return new ResetPasswordCommandHandler(
            refreshTokenRepository.Object,
            unitOfWork.Object,
            userRepository.Object,
            userVerificationRepository.Object,
            dateTimeProvider.Object,
            passwordHasher.Object
        );
    }

    private static UserVerification CreateUserVerification(
        Ulid userId,
        string code = "123456",
        DateTimeOffset? createdAt = null,
        DateTimeOffset? expiresAt = null)
    {
        var creationTimestamp = createdAt ?? Timestamp.AddMinutes(-1);

        return UserVerification.Create(
            id: Ulid.NewUlid(),
            userId: userId,
            purpose: UserVerificationPurpose.PasswordReset,
            verificationCode: Code.From(code),
            expiresAt: expiresAt ?? Timestamp.AddMinutes(5),
            createdAt: creationTimestamp
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new ResetPasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRefreshTokenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ResetPasswordCommandHandler(
                default!,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ResetPasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                default!,
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ResetPasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserVerificationRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ResetPasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ResetPasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                default!,
                Mock.Of<IPasswordHasher>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenPasswordHasherIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new ResetPasswordCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Handle")]
    public async Task Handle_ShouldResetPassword_WhenCommandIsValid()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);
        var newPasswordHash = new string('b', 64);
        var oldPasswordHash = user.PasswordHash.Value;

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.Is<Code>(code => code.IsMatch(command.Code)), It.IsAny<CancellationToken>())).ReturnsAsync(userVerification);
        mockPasswordHasher.Setup(x => x.VerifyPassword(command.NewPassword, oldPasswordHash)).Returns(false);
        mockPasswordHasher.Setup(x => x.HashPassword(command.NewPassword)).Returns(newPasswordHash);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.Equal(newPasswordHash, user.PasswordHash.Value);
        Assert.Equal(Timestamp, user.UpdatedAt);
        Assert.Equal(Timestamp, userVerification.ConsumedAt);
        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), CancellationToken.None), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.Is<Code>(code => code.IsMatch(command.Code)), CancellationToken.None), Times.Once);
        mockPasswordHasher.Verify(x => x.VerifyPassword(command.NewPassword, oldPasswordHash), Times.Once);
        mockPasswordHasher.Verify(x => x.HashPassword(command.NewPassword), Times.Once);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(userVerification, CancellationToken.None), Times.Once);
        mockUserRepository.Verify(x => x.UpdateAsync(user, CancellationToken.None), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(user.Id, Timestamp, CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), token)).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.Is<Code>(code => code.IsMatch(command.Code)), token)).ReturnsAsync(userVerification);
        mockPasswordHasher.Setup(x => x.VerifyPassword(command.NewPassword, user.PasswordHash.Value)).Returns(false);
        mockPasswordHasher.Setup(x => x.HashPassword(command.NewPassword)).Returns(new string('b', 64));

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.Is<Code>(code => code.IsMatch(command.Code)), token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(userVerification, token), Times.Once);
        mockUserRepository.Verify(x => x.UpdateAsync(user, token), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(user.Id, Timestamp, token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenUserDoesNotExist()
    {
        var command = CreateCommand();
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<Code>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenUserIsDeleted()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, Timestamp);
        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<Code>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("missing")]
    [InlineData("invalid-code")]
    [InlineData("consumed")]
    [InlineData("expired")]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenVerificationIsInvalid(string scenario)
    {
        var command = CreateCommand(code: scenario == "invalid-code" ? "654321" : "123456");
        var user = UserTestFactory.CreateUser();
        var userVerification = scenario switch
        {
            "missing" => null,
            "invalid-code" => null,
            "expired" => CreateUserVerification(user.Id, expiresAt: Timestamp),
            _ => CreateUserVerification(user.Id)
        };

        if (scenario == "consumed")
        {
            userVerification!.Consume(Timestamp);
        }

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.Is<Code>(code => code.IsMatch(command.Code)), It.IsAny<CancellationToken>())).ReturnsAsync(userVerification);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await Assert.ThrowsAsync<InvalidOrExpiredVerificationCodeException>(() => handler.Handle(command, CancellationToken.None));

        mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(It.IsAny<Ulid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/ResetPasswordCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenNewPasswordIsSameAsOldPassword()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);
        var oldPasswordHash = user.PasswordHash.Value;

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockPasswordHasher = new Mock<IPasswordHasher>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.Is<Code>(code => code.IsMatch(command.Code)), It.IsAny<CancellationToken>())).ReturnsAsync(userVerification);
        mockPasswordHasher.Setup(x => x.VerifyPassword(command.NewPassword, oldPasswordHash)).Returns(true);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockPasswordHasher);

        await Assert.ThrowsAsync<NewPasswordSameAsOldPasswordException>(() => handler.Handle(command, CancellationToken.None));

        Assert.Null(userVerification.ConsumedAt);
        Assert.Equal(oldPasswordHash, user.PasswordHash.Value);
        mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockRefreshTokenRepository.Verify(x => x.RevokeAllForUserAsync(It.IsAny<Ulid>(), It.IsAny<DateTimeOffset>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
