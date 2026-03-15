using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Users.Commands.SetUserActivationStatus;
using Common.Tests.Factories;
using Domain.Entities;
using Moq;

namespace Application.Tests.Users.Commands.SetUserActivationStatus;

[Trait("Layer", "Application")]
public sealed class SetUserActivationStatusCommandHandlerTests
{
  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Constructor")]
  public void Constructor_ShouldInitialize_WithValidDependencies()
  {
    // Act and Assert
    var exception = Record.Exception(() =>
      new SetUserActivationStatusCommandHandler(
        Mock.Of<IUserRepository>(),
        Mock.Of<IDateTimeProvider>()
      )
    );

    Assert.Null(exception);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() =>
      new SetUserActivationStatusCommandHandler(
        default!, // Force non-nullable UserRepository for testing
        Mock.Of<IDateTimeProvider>()
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() =>
      new SetUserActivationStatusCommandHandler(
        Mock.Of<IUserRepository>(),
        default! // Force non-nullable DateTimeProvider for testing
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Handle")]
  public async Task Handle_ShouldSetUserActivationStatus_WhenCommandIsValid()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockUserRepository
      .Setup(x => x.SetIsActiveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    var handler = new SetUserActivationStatusCommandHandler(mockUserRepository.Object, mockDateTimeProvider.Object);

    var command = new SetUserActivationStatusCommand(user.Id, false); // Is active by default in UserTestFactory

    // Act
    await handler.Handle(command, CancellationToken.None);

    // Assert
    mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
    mockUserRepository.Verify(x => x.SetIsActiveAsync(user, It.IsAny<CancellationToken>()), Times.Once);

    Assert.False(user.IsActive);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsNotFound()
  {
    // Arrange
    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((User?)null);

    mockUserRepository
      .Setup(x => x.SetIsActiveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    var handler = new SetUserActivationStatusCommandHandler(mockUserRepository.Object, mockDateTimeProvider.Object);

    var command = new SetUserActivationStatusCommand(Ulid.NewUlid(), false);

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
    mockUserRepository.Verify(x => x.SetIsActiveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);

    Assert.NotNull(exception);
    Assert.IsType<UserNotFoundException>(exception);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();
    user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // Deleted 2 Days from creation date

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockUserRepository
      .Setup(x => x.SetIsActiveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    var handler = new SetUserActivationStatusCommandHandler(mockUserRepository.Object, mockDateTimeProvider.Object);

    var command = new SetUserActivationStatusCommand(user.Id, false);

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserAlreadyDeletedException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
    mockUserRepository.Verify(x => x.SetIsActiveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);

    Assert.NotNull(exception);
    Assert.IsType<UserAlreadyDeletedException>(exception);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserActivationStatusCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserActivationStatusNotChanged()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockUserRepository
      .Setup(x => x.SetIsActiveAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    var handler = new SetUserActivationStatusCommandHandler(mockUserRepository.Object, mockDateTimeProvider.Object);

    var command = new SetUserActivationStatusCommand(user.Id, user.IsActive);

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserActivationStatusNotChangedException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
    mockUserRepository.Verify(x => x.SetIsActiveAsync(user, It.IsAny<CancellationToken>()), Times.Never);

    Assert.NotNull(exception);
    Assert.Equal(user.IsActive, command.IsActive);
    Assert.IsType<UserActivationStatusNotChangedException>(exception);
  }
}