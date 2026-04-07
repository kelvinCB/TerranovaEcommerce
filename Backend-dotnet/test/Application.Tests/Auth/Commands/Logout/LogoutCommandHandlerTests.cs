using Application.Auth.Commands.Logout;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Common.Tests.Factories;
using Moq;
using MediatR;

namespace Application.Tests.Auth.Commands.Logout;

[Trait("Layer", "Application")]
public sealed class LogoutCommandHandlerTests
{
    private readonly DateTimeOffset _timestamp = new(2026, 1, 5, 0, 0, 0, TimeSpan.Zero);

    private static LogoutCommand CreateCommand(string refreshToken = "plain-refresh-token")
    {
        return new LogoutCommand(refreshToken);
    }

    private LogoutCommandHandler CreateHandler(
        Mock<IRefreshTokenRepository> refreshTokenRepository,
        Mock<IUnitOfWork> unitOfWork,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<ITokenHashService> tokenHashService)
    {
        return new LogoutCommandHandler(
            refreshTokenRepository.Object,
            unitOfWork.Object,
            dateTimeProvider.Object,
            tokenHashService.Object
        );
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        var exception = Record.Exception(() =>
            new LogoutCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRefreshTokenRepositoryIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LogoutCommandHandler(
                default!,
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LogoutCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                default!,
                Mock.Of<IDateTimeProvider>(),
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LogoutCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                default!,
                Mock.Of<ITokenHashService>()
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenTokenHashServiceIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LogoutCommandHandler(
                Mock.Of<IRefreshTokenRepository>(),
                Mock.Of<IUnitOfWork>(),
                Mock.Of<IDateTimeProvider>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Handle")]
    public async Task Handle_ShouldRevokeRefreshToken_WhenRequestIsValid()
    {
        var command = CreateCommand();
        var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
        var tokenHash = "hashed-refresh-token";

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService
            .Setup(x => x.HashToken(command.RefreshToken))
            .Returns(tokenHash);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(tokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync(refreshToken);
        mockDateTimeProvider
            .Setup(x => x.Timestamp)
            .Returns(_timestamp);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockDateTimeProvider,
            mockTokenHashService);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.True(refreshToken.IsRevoked);
        Assert.Equal(_timestamp, refreshToken.RevokedAt);

        mockTokenHashService.Verify(x => x.HashToken(command.RefreshToken), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.GetByTokenHashAsync(tokenHash, CancellationToken.None), Times.Once);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(refreshToken, CancellationToken.None), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Handle")]
    public async Task Handle_ShouldPropagateCancellationToken()
    {
        using var cancellationTokenSource = new CancellationTokenSource();
        var token = cancellationTokenSource.Token;
        var command = CreateCommand();
        var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
        var tokenHash = "hashed-refresh-token";

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(tokenHash);
        mockRefreshTokenRepository.Setup(x => x.GetByTokenHashAsync(tokenHash, token)).ReturnsAsync(refreshToken);
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(_timestamp);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockDateTimeProvider,
            mockTokenHashService);

        await handler.Handle(command, token);

        mockRefreshTokenRepository.Verify(x => x.GetByTokenHashAsync(tokenHash, token), Times.Once);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(refreshToken, token), Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(token), Times.Once);
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenRefreshTokenDoesNotExist()
    {
        var command = CreateCommand();
        var tokenHash = "hashed-refresh-token";

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService
            .Setup(x => x.HashToken(command.RefreshToken))
            .Returns(tokenHash);
        mockRefreshTokenRepository
            .Setup(x => x.GetByTokenHashAsync(tokenHash, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Domain.Entities.RefreshToken?)null);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockDateTimeProvider,
            mockTokenHashService);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Auth", "Commands/Logout/LogoutCommandHandler/Handle")]
    public async Task Handle_ShouldReturnUnit_WhenRefreshTokenIsAlreadyRevoked()
    {
        var command = CreateCommand();
        var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
        var tokenHash = "hashed-refresh-token";
        var revokedAt = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
        refreshToken.Revoke(revokedAt);

        var mockRefreshTokenRepository = new Mock<IRefreshTokenRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var mockDateTimeProvider = new Mock<IDateTimeProvider>();
        var mockTokenHashService = new Mock<ITokenHashService>();

        mockTokenHashService.Setup(x => x.HashToken(command.RefreshToken)).Returns(tokenHash);
        mockRefreshTokenRepository.Setup(x => x.GetByTokenHashAsync(tokenHash, It.IsAny<CancellationToken>())).ReturnsAsync(refreshToken);
        mockDateTimeProvider.Setup(x => x.Timestamp).Returns(_timestamp);

        var handler = CreateHandler(
            mockRefreshTokenRepository,
            mockUnitOfWork,
            mockDateTimeProvider,
            mockTokenHashService);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Equal(Unit.Value, result);
        Assert.True(refreshToken.IsRevoked);
        Assert.Equal(revokedAt, refreshToken.RevokedAt);
        mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
        mockRefreshTokenRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.RefreshToken>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}