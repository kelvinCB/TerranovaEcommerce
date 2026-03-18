using Moq;
using Application.Common.Abstractions.Services;
using Application.Common.Abstractions.Persistence;
using Application.Users.Commands.SetUserRole;
using Common.Tests.Factories;
using Domain.Entities;
using Application.Common.Exceptions;

namespace Application.Tests.Users.Commands.SetUserRole;

[Trait("Layer", "Application")]
public class SetUserRoleCommandHandlerTests
{
  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Constructor")]
  public void Constructor_ShouldInitialize_WithValidDependencies()
  {
    // Act and Assert
    var exception = Record.Exception(() => 
      new SetUserRoleCommandHandler(
        Mock.Of<IUserRepository>(),
        Mock.Of<IRoleRepository>(),
        Mock.Of<IUserRoleRepository>(),
        Mock.Of<IDateTimeProvider>()
      )
    );

    Assert.Null(exception);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SetUserRoleCommandHandler(
        default!, // Force non-nullable UserRepository for testing
        Mock.Of<IRoleRepository>(),
        Mock.Of<IUserRoleRepository>(),
        Mock.Of<IDateTimeProvider>()
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenRoleRepositoryIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SetUserRoleCommandHandler(
        Mock.Of<IUserRepository>(),
        default!, // Force non-nullable RoleRepository for testing
        Mock.Of<IUserRoleRepository>(),
        Mock.Of<IDateTimeProvider>()
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SetUserRoleCommandHandler(
        Mock.Of<IUserRepository>(),
        Mock.Of<IRoleRepository>(),
        default!, // Force non-nullable UserRoleRepository for testing
        Mock.Of<IDateTimeProvider>()
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
      new SetUserRoleCommandHandler(
        Mock.Of<IUserRepository>(),
        Mock.Of<IRoleRepository>(),
        Mock.Of<IUserRoleRepository>(),
        default! // Force non-nullable DateTimeProvider for testing
      )
    );
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Handle")]
  public async Task Handle_ShouldSetUserRole_WhenParametersAreValid()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockRoleRepository = new Mock<IRoleRepository>();
    var mockUserRoleRepository = new Mock<IUserRoleRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockRoleRepository
      .Setup(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(true);

    mockUserRoleRepository
      .Setup(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(false);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)); // 1 Day from creation date

    mockUserRoleRepository
      .Setup(x => x.RegisterAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()));

    var handler = new SetUserRoleCommandHandler(
      mockUserRepository.Object,
      mockRoleRepository.Object,
      mockUserRoleRepository.Object,
      mockDateTimeProvider.Object
    );

    // Act
    await handler.Handle(new SetUserRoleCommand(user.Id, Ulid.NewUlid()), CancellationToken.None);

    // Assert
    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockRoleRepository.Verify(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRoleRepository.Verify(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRoleRepository.Verify(x => x.RegisterAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Once);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
  {
    // Arrange
    var mockUserRepository = new Mock<IUserRepository>();
    var mockRoleRepository = new Mock<IRoleRepository>();
    var mockUserRoleRepository = new Mock<IUserRoleRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((User?)null);

    var handler = new SetUserRoleCommandHandler(
      mockUserRepository.Object,
      mockRoleRepository.Object,
      mockUserRoleRepository.Object,
      mockDateTimeProvider.Object
    );

    // Act and Assert
    await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(new SetUserRoleCommand(Ulid.NewUlid(), Ulid.NewUlid()), CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockRoleRepository.Verify(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUserRoleRepository.Verify(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUserRoleRepository.Verify(x => x.RegisterAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsSoftDeleted()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)); // Deleted 1 Day from creation date

    var mockUserRepository = new Mock<IUserRepository>();
    var mockRoleRepository = new Mock<IRoleRepository>();
    var mockUserRoleRepository = new Mock<IUserRoleRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    var handler = new SetUserRoleCommandHandler(
      mockUserRepository.Object,
      mockRoleRepository.Object,
      mockUserRoleRepository.Object,
      mockDateTimeProvider.Object
    );

    // Act and Assert
    await Assert.ThrowsAsync<UserAlreadyDeletedException>(() => handler.Handle(new SetUserRoleCommand(user.Id, Ulid.NewUlid()), CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockRoleRepository.Verify(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUserRoleRepository.Verify(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUserRoleRepository.Verify(x => x.RegisterAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenRoleDoesNotExist()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockRoleRepository = new Mock<IRoleRepository>();
    var mockUserRoleRepository = new Mock<IUserRoleRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockRoleRepository
      .Setup(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(false);

    var handler = new SetUserRoleCommandHandler(
      mockUserRepository.Object,
      mockRoleRepository.Object,
      mockUserRoleRepository.Object,
      mockDateTimeProvider.Object
    );

    // Act and Assert
    await Assert.ThrowsAsync<RoleNotFoundException>(() => handler.Handle(new SetUserRoleCommand(user.Id, Ulid.NewUlid()), CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockRoleRepository.Verify(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRoleRepository.Verify(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUserRoleRepository.Verify(x => x.RegisterAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact]
  [Trait("Users", "Commands/SetUserRole/SetUserRoleCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserAlreadyHasRole()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockRoleRepository = new Mock<IRoleRepository>();
    var mockUserRoleRepository = new Mock<IUserRoleRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockRoleRepository
      .Setup(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(true);

    mockUserRoleRepository
      .Setup(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(true);

    var handler = new SetUserRoleCommandHandler(
      mockUserRepository.Object,
      mockRoleRepository.Object,
      mockUserRoleRepository.Object,
      mockDateTimeProvider.Object
    );

    // Act and Assert
    await Assert.ThrowsAsync<UserAlreadyHasRoleException>(() => handler.Handle(new SetUserRoleCommand(user.Id, Ulid.NewUlid()), CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockRoleRepository.Verify(x => x.ExistsByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRoleRepository.Verify(x => x.ExistsUserRoleAsync(It.IsAny<Ulid>(), It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRoleRepository.Verify(x => x.RegisterAsync(It.IsAny<UserRole>(), It.IsAny<CancellationToken>()), Times.Never);
  }
}