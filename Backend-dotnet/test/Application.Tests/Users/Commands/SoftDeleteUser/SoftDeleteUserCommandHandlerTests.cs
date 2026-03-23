using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Users.Commands.SoftDeleteUser;
using Common.Tests.Factories;
using Domain.Entities;
using Moq;

namespace Application.Tests.Users.Commands.SoftDeleteUser;

[Trait("Layer", "Application")]
public sealed class SoftDeleteUserCommandHandlerTests
{
  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Constructor")]
  public void Constructor_ShouldInitialize_WithValidDependencies()
  {
    // Act and Assert
    var exception = Record.Exception(() => 
      new SoftDeleteUserCommandHandler(
        Mock.Of<IUserRepository>(),
        Mock.Of<IDateTimeProvider>(),
        Mock.Of<IUnitOfWork>()
      )
    );

    Assert.Null(exception);
  }

  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SoftDeleteUserCommandHandler(
        default!, // Force non-nullable UserRepository for testing
        Mock.Of<IDateTimeProvider>(),
        Mock.Of<IUnitOfWork>()
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SoftDeleteUserCommandHandler(
        Mock.Of<IUserRepository>(),
        default!, // Force non-nullable DateTimeProvider for testing
        Mock.Of<IUnitOfWork>()
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SoftDeleteUserCommandHandler(
        Mock.Of<IUserRepository>(),
        Mock.Of<IDateTimeProvider>(),
        default! // Force non-nullable UnitOfWork for testing
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Handle")]
  public async Task Handle_ShouldSoftDeleteUser_WhenUserExists()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockUserRepository
      .Setup(x => x.SoftDeleteAsync(user, It.IsAny<CancellationToken>()));

    mockDateTimeProvider
        .Setup(x => x.Timestamp)
        .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    mockUnitOfWork
      .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var handler = new SoftDeleteUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new SoftDeleteUserCommand(user.Id);

    // Act
    var result = await handler.Handle(command, CancellationToken.None);

    // Assert
    mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Once);
    mockUserRepository.Verify(x => x.SoftDeleteAsync(user, It.IsAny<CancellationToken>()), Times.Once);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    Assert.True(user.IsDeleted);
  }

  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsNotFound()
  {
    // Arrange
    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((User?)null);

    mockUserRepository
      .Setup(x => x.SoftDeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()));

    mockDateTimeProvider
        .Setup(x => x.Timestamp)
        .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    mockUnitOfWork
      .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var handler = new SoftDeleteUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new SoftDeleteUserCommand(Ulid.NewUlid());

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
    mockUserRepository.Verify(x => x.SoftDeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    Assert.NotNull(exception);
    Assert.IsType<UserNotFoundException>(exception);
    Assert.Contains(command.Id.ToString(), exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("Users", "Commands/SoftDeleteUser/SoftDeleteUserCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsAlreadyDeleted()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();
    user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // Deleted 2 Days from creation date

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockUserRepository
      .Setup(x => x.SoftDeleteAsync(user, It.IsAny<CancellationToken>()));

    mockDateTimeProvider
        .Setup(x => x.Timestamp)
        .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero)); // 2 Days from creation date

    mockUnitOfWork
      .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var handler = new SoftDeleteUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new SoftDeleteUserCommand(user.Id);

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserAlreadyDeletedException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()), Times.Once);
    mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
    mockUserRepository.Verify(x => x.SoftDeleteAsync(user, It.IsAny<CancellationToken>()), Times.Never);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    Assert.NotNull(exception);
    Assert.IsType<UserAlreadyDeletedException>(exception);
    Assert.Contains(command.Id.ToString(), exception.Message, StringComparison.OrdinalIgnoreCase);
  }
}