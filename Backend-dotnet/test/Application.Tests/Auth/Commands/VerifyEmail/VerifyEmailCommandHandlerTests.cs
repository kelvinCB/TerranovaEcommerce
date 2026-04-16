using Application.Auth.Commands.VerifyEmail;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Common.Tests.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;

namespace Application.Tests.Auth.Commands.VerifyEmail;

[Trait("Layer", "Application")]
public sealed class VerifyEmailCommandHandlerTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static VerifyEmailCommand CreateCommand(string email = "test@example.com", string code = "123456")
    {
        return new VerifyEmailCommand(email, code);
    }

    private static VerifyEmailCommandHandler CreateHandler(
        Mock<IUnitOfWork> unitOfWork,
        Mock<IUserRepository> userRepository,
        Mock<IUserVerificationRepository> userVerificationRepository,
        Mock<IDateTimeProvider> dateTimeProvider)
    {
        return new VerifyEmailCommandHandler(
            unitOfWork.Object,
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
            purpose: UserVerificationPurpose.EmailVerify,
            verificationCode: Code.From(code),
            expiresAt: expiresAt ?? Timestamp.AddMinutes(5),
            createdAt: creationTimestamp
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new VerifyEmailCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyEmailCommandHandler(
                default!,
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyEmailCommandHandler(
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserVerificationRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyEmailCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new VerifyEmailCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldVerifyEmailAndConsumeVerification_WhenCommandIsValid()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.Is<Code>(c => c.IsMatch(command.Code)), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userVerification);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        Assert.True(user.IsEmailAddressVerified);
        Assert.Equal(Timestamp, user.UpdatedAt);
        Assert.Equal(Timestamp, userVerification.ConsumedAt);
        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), CancellationToken.None), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.Is<Code>(c => c.IsMatch(command.Code)), CancellationToken.None), Times.Once);
        mockUserRepository.Verify(x => x.UpdateAsync(user, CancellationToken.None), Times.Once);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(userVerification, CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), token)).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<Code>(), token))
            .ReturnsAsync(userVerification);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<Code>(), token), Times.Once);
        mockUserRepository.Verify(x => x.UpdateAsync(user, token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(userVerification, token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldReturnTrue_WhenEmailIsAlreadyVerified()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        user.SetIsEmailAddressVerified(true, Timestamp);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<Code>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenUserDoesNotExist()
    {
        var command = CreateCommand();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<Code>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenUserIsDeleted()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, Timestamp);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockUserVerificationRepository.Verify(x => x.GetActiveByUserIdPurposeAndCodeAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<Code>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenVerificationDoesNotExist()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<Code>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserVerification?)null);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenVerificationIsExpired()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id, expiresAt: Timestamp);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<Code>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userVerification);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        Assert.False(user.IsEmailAddressVerified);
        Assert.Null(userVerification.ConsumedAt);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/VerifyEmailCommandHandler/Handle")]
    public async Task Handle_ShouldReturnFalse_WhenVerificationIsAlreadyConsumed()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var userVerification = CreateUserVerification(user.Id);
        userVerification.Consume(Timestamp);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockUserVerificationRepository
            .Setup(x => x.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<Code>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(userVerification);

        var handler = CreateHandler(mockUnitOfWork, mockUserRepository, mockUserVerificationRepository, mockDateTimeProvider);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
        Assert.False(user.IsEmailAddressVerified);
        mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.UpdateAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
