using Application.Auth.Commands.VerifyPasswordResetCode;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Common.Tests.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Auth.Commands.VerifyPasswordResetCode;

[Trait("Layer", "Application")]
public sealed class VerifyPasswordResetCodeCommandHandlerTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static VerifyPasswordResetCodeCommand CreateCommand(
        string email = "test@example.com",
        string code = "123456")
    {
        return new VerifyPasswordResetCodeCommand(email, code);
    }

    private static VerifyPasswordResetCodeCommandHandler CreateHandler(
        Mock<IUserRepository> userRepository,
        Mock<IUserVerificationRepository> userVerificationRepository,
        Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new VerifyPasswordResetCodeCommandHandler(
            userRepository.Object,
            userVerificationRepository.Object,
            dateTimeProvider.Object
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
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new VerifyPasswordResetCodeCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyPasswordResetCodeCommandHandler(
                default!,
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserVerificationRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyPasswordResetCodeCommandHandler(
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyPasswordResetCodeCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnTrue_WhenCodeIsValid()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userVerification);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.Null(userVerification.ConsumedAt);
        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), CancellationToken.None), Times.Once);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, CancellationToken.None), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), token)).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, token)).ReturnsAsync(userVerification);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        var command = CreateCommand();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdAndPurposeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenUserIsDeleted()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, Timestamp);
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdAndPurposeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenVerificationDoesNotExist()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserVerification?)null);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenCodeDoesNotMatch()
    {
        var command = CreateCommand(code: "654321");
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id, code: "123456");
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>())).ReturnsAsync(userVerification);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        Assert.Null(userVerification.ConsumedAt);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenVerificationIsExpired()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id, expiresAt: Timestamp);
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>())).ReturnsAsync(userVerification);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        Assert.Null(userVerification.ConsumedAt);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyPasswordResetCodeCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenVerificationIsAlreadyConsumed()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);
        userVerification.Consume(Timestamp);
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.GetActiveByUserIdAndPurposeAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>())).ReturnsAsync(userVerification);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);

        var handler = CreateHandler(mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
