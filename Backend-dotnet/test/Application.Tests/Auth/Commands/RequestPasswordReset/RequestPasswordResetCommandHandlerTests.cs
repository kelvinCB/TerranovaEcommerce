using Application.Auth.Commands.RequestPasswordReset;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Common.Tests.Factories;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
using Moq;

namespace Application.Tests.Auth.Commands.RequestPasswordReset;

[Trait("Layer", "Application")]
public sealed class RequestPasswordResetCommandHandlerTests
{
    private static readonly DateTimeOffset Timestamp = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static RequestPasswordResetCommand CreateCommand(string emailAddress = "test@example.com")
    {
        return new RequestPasswordResetCommand(emailAddress);
    }

    private static RequestPasswordResetCommandHandler CreateHandler(
        Mock<IUnitOfWork> unitOfWork,
        Mock<IUserRepository> userRepository,
        Mock<IUserVerificationRepository> userVerificationRepository,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<IIdGenerator> idGenerator,
        Mock<IVerificationCodeGenerator> verificationCodeGenerator)
    {
        return new RequestPasswordResetCommandHandler(
            unitOfWork.Object,
            userRepository.Object,
            userVerificationRepository.Object,
            dateTimeProvider.Object,
            idGenerator.Object,
            verificationCodeGenerator.Object
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new RequestPasswordResetCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestPasswordResetCommandHandler(
                default!,
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestPasswordResetCommandHandler(
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserVerificationRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestPasswordResetCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestPasswordResetCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                default!,
                Mock.Of<IIdGenerator>(),
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenIdGeneratorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestPasswordResetCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                default!,
                Mock.Of<IVerificationCodeGenerator>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenVerificationCodeGeneratorIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new RequestPasswordResetCommandHandler(
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserVerificationRepository>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<IIdGenerator>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Handle")]
    public async Task Handle_ShouldCreateUserVerification_WhenUserExistsAndHasNoActiveVerification()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();
        var verificationId = Ulid.NewUlid();
        var verificationCode = Code.From("123456");

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockUserVerificationRepository
            .Setup(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(verificationId);
        mockVerificationCodeGenerator.Setup(x => x.Generate()).Returns(verificationCode);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), CancellationToken.None), Times.Once);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.PasswordReset, CancellationToken.None), Times.Once);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Once);
        mockVerificationCodeGenerator.Verify(x => x.Generate(), Times.Once);
        mockUserVerificationRepository.Verify(x => x.AddAsync(
            It.Is<UserVerification>(v =>
                v.Id == verificationId &&
                v.UserId == user.Id &&
                v.Purpose == UserVerificationPurpose.PasswordReset &&
                v.VerificationCode == verificationCode &&
                v.CreatedAt == Timestamp &&
                v.ExpiresAt == Timestamp.AddMinutes(5) &&
                v.ConsumedAt == null),
            CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Handle")]
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
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository.Setup(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), token)).ReturnsAsync(user);
        mockUserVerificationRepository.Setup(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.PasswordReset, token)).ReturnsAsync(false);
        mockDateTimeProvider.SetupGet(x => x.Timestamp).Returns(Timestamp);
        mockIdGenerator.Setup(x => x.NewUlid()).Returns(Ulid.NewUlid());
        mockVerificationCodeGenerator.Setup(x => x.Generate()).Returns(Code.From("123456"));

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockVerificationCodeGenerator);

        await handler.Handle(command, token);

        mockUserRepository.Verify(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.PasswordReset, token), Times.Once);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenUserDoesNotExist()
    {
        var command = CreateCommand();

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Handle")]
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
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockUserVerificationRepository.Verify(x => x.ExistsActiveVerificationAsync(It.IsAny<Ulid>(), It.IsAny<UserVerificationPurpose>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/RequestPasswordResetCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenActiveVerificationAlreadyExists()
    {
        var command = CreateCommand();
        var user = UserTestFactory.CreateUser();

        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserVerificationRepository = new Mock<IUserVerificationRepository>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockIdGenerator = new Mock<IIdGenerator>();
        var mockVerificationCodeGenerator = new Mock<IVerificationCodeGenerator>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(Email.Create(command.EmailAddress), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        mockUserVerificationRepository
            .Setup(x => x.ExistsActiveVerificationAsync(user.Id, UserVerificationPurpose.PasswordReset, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = CreateHandler(
            mockUnitOfWork,
            mockUserRepository,
            mockUserVerificationRepository,
            mockDateTimeProvider,
            mockIdGenerator,
            mockVerificationCodeGenerator);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockIdGenerator.Verify(x => x.NewUlid(), Times.Never);
        mockVerificationCodeGenerator.Verify(x => x.Generate(), Times.Never);
        mockUserVerificationRepository.Verify(x => x.AddAsync(It.IsAny<UserVerification>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}
