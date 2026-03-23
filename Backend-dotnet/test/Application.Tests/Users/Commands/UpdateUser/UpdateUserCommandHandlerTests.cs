using Application.Users.Commands.UpdateUser;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Domain.Entities;
using Common.Tests.Factories;
using Moq;
using Application.Common.Exceptions;

namespace Application.Tests.Users.Commands.UpdateUser;

[Trait("Layer", "Application")]
public sealed class UpdateUserCommandHandlerTests()
{
  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Constructor")]
  public void Constructor_ShouldInitialize_WithValidDependencies()
  {
    // Act and Assert
    var exception = Record.Exception(() => 
        new UpdateUserCommandHandler(
          Mock.Of<IUserRepository>(),
          Mock.Of<IDateTimeProvider>(),
          Mock.Of<IUnitOfWork>()
        )
      );

    Assert.Null(exception);
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
        new UpdateUserCommandHandler(
          default!, // Force non-nullable UserRepository for testing
          Mock.Of<IDateTimeProvider>(),
          Mock.Of<IUnitOfWork>()
        )
      );
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
        new UpdateUserCommandHandler(
          Mock.Of<IUserRepository>(),
          default!, // Force non-nullable DateTimeProvider for testing
          Mock.Of<IUnitOfWork>()
        )
      );
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Constructor")]
  public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
  {
    // Act and Assert
    Assert.Throws<ArgumentNullException>(() => 
        new UpdateUserCommandHandler(
          Mock.Of<IUserRepository>(),
          Mock.Of<IDateTimeProvider>(),
          default! // Force non-nullable UnitOfWork for testing
        )
      );
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Handle")]
  public async Task Handle_ShouldUpdateUser_WhenParametersAreValid()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero));

    mockUnitOfWork
      .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var handler = new UpdateUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new UpdateUserCommand(
      user.Id,
      "Briangel", // David is the original first name
      "Santana Calcanio", // Calcanio Hernandez is the original last name
      'M',
      new DateOnly(2001, 1, 1)
    );

    // Act
    await handler.Handle(command, CancellationToken.None);

    // Assert
    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    Assert.Equal(command.FirstName, user.FirstName);
    Assert.Equal(command.LastName, user.LastName);
    Assert.Equal(command.Gender, user.Gender);
    Assert.Equal(command.BirthDate, user.BirthDate);
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Handle")]
  public async Task Handle_ShouldUpdateUser_WhenParametersAreNull()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();
    var originalFirstName = user.FirstName;
    var originalLastName = user.LastName;
    var originalGender = user.Gender;
    var originalBirthDate = user.BirthDate;

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero));

    mockUnitOfWork
      .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var handler = new UpdateUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new UpdateUserCommand(
      user.Id,
      null, // David is the original first name
      null, // Calcanio Hernandez is the original last name
      null, // M is the original gender
      null // 01/01/2001 is the original birth date
    );

    // Act
    await handler.Handle(command, CancellationToken.None);

    // Assert
    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

    Assert.Equal(originalFirstName, user.FirstName);
    Assert.Equal(originalLastName, user.LastName);
    Assert.Equal(originalGender, user.Gender);
    Assert.Equal(originalBirthDate, user.BirthDate);
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsNotFound()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync((User?)null);

    var handler = new UpdateUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new UpdateUserCommand(
      user.Id,
      "Briangel", // David is the original first name
      "Santana Calcanio", // Calcanio Hernandez is the original last name
      'M',
      new DateOnly(2001, 1, 1)
    );

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    Assert.Contains($"User with id {user.Id} was not found", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();
    user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero));

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    var handler = new UpdateUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object
    );

    var command = new UpdateUserCommand(
      user.Id,
      "Briangel", // David is the original first name
      "Santana Calcanio", // Calcanio Hernandez is the original last name
      'M',
      new DateOnly(2001, 1, 1)
    );

    // Act and Assert
    var exception = await Assert.ThrowsAsync<UserAlreadyDeletedException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    Assert.Contains($"User with id {user.Id} was already deleted", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("Users", "Commands/UpdateUser/UpdateUserCommandHandler/Handle")]
  public async Task Handle_ShouldThrowException_WhenDateTimeProviderIsNotUtc()
  {
    // Arrange
    var user = UserTestFactory.CreateUser();

    var mockUserRepository = new Mock<IUserRepository>();
    var mockDateTimeProvider = new Mock<IDateTimeProvider>();
    var mockUnitOfWork = new Mock<IUnitOfWork>();

    mockUserRepository
      .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
      .ReturnsAsync(user);

    mockDateTimeProvider
      .Setup(x => x.Timestamp)
      .Returns(new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4))); // It's not UTC

    mockUnitOfWork
      .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
      .Returns(Task.CompletedTask);

    var handler = new UpdateUserCommandHandler(
      mockUserRepository.Object,
      mockDateTimeProvider.Object,
      mockUnitOfWork.Object);

    var command = new UpdateUserCommand(
      user.Id,
      "Briangel", // David is the original first name
      "Santana Calcanio", // Calcanio Hernandez is the original last name
      'M',
      new DateOnly(2001, 1, 1)
    );

    // Act and Assert
    var exception = await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(command, CancellationToken.None));

    mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
    mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);

    Assert.Contains("Timestamp must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }
}