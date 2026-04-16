using Application.Auth.Commands.RequestEmailVerification;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Verification;
using Common.Tests.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Moq;

namespace Application.Tests.Auth.Commands.RequestEmailVerification;

[Trait("Layer", "Application")]
public sealed class RequestEmailVerificationCommandHandlerTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static RequestEmailVerificationCommand CreateCommand(string email = "test@example.com")
    {
        return new RequestEmailVerificationCommand(email);
    }

    private static RequestEmailVerificationCommandHandler CreateHandler(
        Mock<IUnitOfWork> unitOfWork,
        Mock<IUserRepository> userRepository,
        Mock<IUserVerificationRepository> userVerificationRepository,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<IIdGenerator> idGenerator,
        Mock<INotificationService> notificationService,
        Mock<IVerificationCodeGenerator> verificationCodeGenerator)
    {
        return new RequestEmailVerificationCommandHandler(
            unitOfWork.Object,
            userRepository.Object,
            userVerificationRepository.Object,
            dateTimeProvider.Object,
            idGenerator.Object,
            notificationService.Object,
            verificationCodeGenerator.Object
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                default!,
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserVerificationRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                default!,
                Mock.Of<IIdGenerator>(),
                Mock.Of<INotificationService>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenIdGeneratorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                default!,
                Mock.Of<INotificationService>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenNotificationServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                default!,
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenVerificationCodeGeneratorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestEmailVerificationCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<INotificationService>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Handle")]
    public async Task Handle_ShouldCreateVerificationAndSendEmail_WhenUserExistsAndHasNoActiveVerification()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var verificationId = Ulid.NewUlid();
        var verificationCode = Code.From("123456");
        var plainTextCode = "123456";
        var generatedCode = new GeneratedVerificationCode(verificationCode, plainTextCode);

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(verificationId);
        mockVerificationCodeGenerator.Setup(x => x.Generate()).Returns(generatedCode);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockNotificationService,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), CancellationToken.None), Times.Once);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.EmailVerify, CancellationToken.None), Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
        mockVerificationCodeGenerator.Verify(x => x.Generate(), Times.Once);
        mockUserVerificationRepository.Verify(x => x.AddAsync(
            It.Is<UserVerification>(v =>
                v.Id == verificationId &&
                v.UserId == user.Id &&
                v.Purpose == UserVerificationPurpose.EmailVerify &&
                v.VerificationCode == verificationCode &&
                v.CreatedAt == Timestamp &&
                v.ExpiresAt == Timestamp.AddMinutes(5) &&
                v.ConsumedAt == null),
            CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        mockNotificationService.Verify(x => x.SendEmailVerificationCodeToEmailAsync(user.EmailAddress.Value, plainTextCode, CancellationToken.None), Times.Once);
        mockNotificationService.Verify(x => x.SendPasswordResetCodeToEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), token)).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.EmailVerify, token)).ReturnsAsync(false);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(Ulid.NewUlid());
        mockVerificationCodeGenerator.Setup(x => x.Generate()).Returns(new GeneratedVerificationCode(Code.From("123456"), "123456"));

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockNotificationService,
            mockVerificationCodeGenerator);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.Email), token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.EmailVerify, token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
        mockNotificationService.Verify(x => x.SendEmailVerificationCodeToEmailAsync(user.EmailAddress.Value, "123456", token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenUserDoesNotExist()
    {
        var command = CreateCommand();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync((User?)null);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockNotificationService,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockNotificationService.Verify(x => x.SendEmailVerificationCodeToEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenUserIsDeleted()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, Timestamp);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockNotificationService,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockNotificationService.Verify(x => x.SendEmailVerificationCodeToEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenEmailIsAlreadyVerified()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        user.SetIsEmailAddressVerified(true, Timestamp);
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockNotificationService,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Never);
        mockVerificationCodeGenerator.Verify(x => x.Generate(), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockNotificationService.Verify(x => x.SendEmailVerificationCodeToEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestEmailVerificationCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenActiveVerificationAlreadyExists()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockNotificationService = new Mock<INotificationService>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.Email), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.EmailVerify, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockNotificationService,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Never);
        mockVerificationCodeGenerator.Verify(x => x.Generate(), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockNotificationService.Verify(x => x.SendEmailVerificationCodeToEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
