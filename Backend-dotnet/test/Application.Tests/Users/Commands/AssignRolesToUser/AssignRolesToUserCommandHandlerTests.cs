using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Users.Commands.AssignRolesToUser;
using Common.Tests.Factories;
using Domain.Entities;
using MediatR;
using Moq;

namespace Application.Tests.Users.Commands.AssignRolesToUser;

[Trait("Layer", "Application")]
public sealed class AssignRolesToUserCommandHandlerTests
{
    private readonly DateTimeOffset _timestamp = new(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);

    private static AssignRolesToUserCommandHandler CreateHandler(
        Mock<IUserRepository> userRepository,
        Mock<IUserRoleRepository> userRoleRepository,
        Mock<IRoleRepository> roleRepository,
        Mock<IDateTimeProvider> dateTimeProvider,
        Mock<IUnitOfWork> unitOfWork)
    {
      return new AssignRolesToUserCommandHandler(
          userRepository.Object,
          userRoleRepository.Object,
          roleRepository.Object,
          dateTimeProvider.Object,
          unitOfWork.Object
      );
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
      // Act and Assert
        var exception = Record.Exception(() =>
          new AssignRolesToUserCommandHandler(
            Mock.Of<IUserRepository>(),
            Mock.Of<IUserRoleRepository>(),
            Mock.Of<IRoleRepository>(),
            Mock.Of<IDateTimeProvider>(),
            Mock.Of<IUnitOfWork>()
          )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
      // Act and Assert
      Assert.Throws<ArgumentNullException>(() =>
        new AssignRolesToUserCommandHandler(
          default!,
          Mock.Of<IUserRoleRepository>(),
          Mock.Of<IRoleRepository>(),
          Mock.Of<IDateTimeProvider>(),
          Mock.Of<IUnitOfWork>()
        )
      );
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
    {
      // Act and Assert
      Assert.Throws<ArgumentNullException>(() =>
        new AssignRolesToUserCommandHandler(
          Mock.Of<IUserRepository>(),
          default!,
          Mock.Of<IRoleRepository>(),
          Mock.Of<IDateTimeProvider>(),
          Mock.Of<IUnitOfWork>()
        )
      );
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRoleRepositoryIsNull()
    {
      // Act and Assert
      Assert.Throws<ArgumentNullException>(() =>
        new AssignRolesToUserCommandHandler(
          Mock.Of<IUserRepository>(),
          Mock.Of<IUserRoleRepository>(),
          default!,
          Mock.Of<IDateTimeProvider>(),
          Mock.Of<IUnitOfWork>()
        )
      );
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenDateTimeProviderIsNull()
    {
      // Act and Assert
      Assert.Throws<ArgumentNullException>(() =>
        new AssignRolesToUserCommandHandler(
          Mock.Of<IUserRepository>(),
          Mock.Of<IUserRoleRepository>(),
          Mock.Of<IRoleRepository>(),
          default!,
          Mock.Of<IUnitOfWork>()
        )
      );
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
      // Act and Assert
      Assert.Throws<ArgumentNullException>(() =>
        new AssignRolesToUserCommandHandler(
          Mock.Of<IUserRepository>(),
          Mock.Of<IUserRoleRepository>(),
          Mock.Of<IRoleRepository>(),
          Mock.Of<IDateTimeProvider>(),
          default!
        )
      );
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Handle")]
    public async Task Handle_ShouldAssignOnlyRolesThatAreNotAlreadyAssigned()
    {
      // Arrange
      var user = UserTestFactory.CreateUser();
      var requestedRoleIds = new[] { Ulid.NewUlid(), Ulid.NewUlid() };
      var alreadyAssignedRoleIds = new[] { requestedRoleIds[0] };
      var command = new AssignRolesToUserCommand(user.Id, requestedRoleIds);

      var mockUserRepository = new Mock<IUserRepository>();
      var mockUserRoleRepository = new Mock<IUserRoleRepository>();
      var mockRoleRepository = new Mock<IRoleRepository>();
      var mockDateTimeProvider = new Mock<IDateTimeProvider>();
      var mockUnitOfWork = new Mock<IUnitOfWork>();

      mockUserRepository
        .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(user);

      mockRoleRepository
        .Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(requestedRoleIds);

      mockUserRoleRepository
        .Setup(x => x.GetAssignedRoleIdsAsync(user.Id, It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(alreadyAssignedRoleIds);

      mockDateTimeProvider
        .Setup(x => x.Timestamp)
        .Returns(_timestamp);

      var handler = CreateHandler(
        mockUserRepository,
        mockUserRoleRepository,
        mockRoleRepository,
        mockDateTimeProvider,
        mockUnitOfWork
      );

      // Act
      var result = await handler.Handle(command, CancellationToken.None);

      // Assert
      Assert.Equal(Unit.Value, result);
      mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(
              It.Is<IReadOnlyCollection<UserRole>>(roles =>
                  roles.Count == 1 &&
                  roles.Single().UserId == user.Id &&
                  roles.Single().RoleId == requestedRoleIds[1] &&
                  roles.Single().CreatedAt == _timestamp),
              CancellationToken.None),
          Times.Once);
      mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Handle")]
    public async Task Handle_ShouldReturnWithoutPersisting_WhenAllRolesAreAlreadyAssigned()
    {
      // Arrange
      var user = UserTestFactory.CreateUser();
      var requestedRoleIds = new[] { Ulid.NewUlid(), Ulid.NewUlid() };
      var command = new AssignRolesToUserCommand(user.Id, requestedRoleIds);

      var mockUserRepository = new Mock<IUserRepository>();
      var mockUserRoleRepository = new Mock<IUserRoleRepository>();
      var mockRoleRepository = new Mock<IRoleRepository>();
      var mockDateTimeProvider = new Mock<IDateTimeProvider>();
      var mockUnitOfWork = new Mock<IUnitOfWork>();

      mockUserRepository
        .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(user);

      mockRoleRepository
        .Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(requestedRoleIds);

      mockUserRoleRepository
        .Setup(x => x.GetAssignedRoleIdsAsync(user.Id, It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(requestedRoleIds);

      var handler = CreateHandler(
        mockUserRepository,
        mockUserRoleRepository,
        mockRoleRepository,
        mockDateTimeProvider,
        mockUnitOfWork
      );

      // Act
      var result = await handler.Handle(command, CancellationToken.None);

      // Assert
      Assert.Equal(Unit.Value, result);
      mockDateTimeProvider.Verify(x => x.Timestamp, Times.Never);
      mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenNoRolesAreProvided()
    {
      // Arrange
      var command = new AssignRolesToUserCommand(Ulid.NewUlid(), []);

      var handler = CreateHandler(
        new Mock<IUserRepository>(),
        new Mock<IUserRoleRepository>(),
        new Mock<IRoleRepository>(),
        new Mock<IDateTimeProvider>(),
        new Mock<IUnitOfWork>()
      );

      // Act and Assert
      await Assert.ThrowsAsync<AtLeastOneRoleMustBeProvidedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
      // Arrange
      var command = new AssignRolesToUserCommand(Ulid.NewUlid(), [Ulid.NewUlid()]);

      var mockUserRepository = new Mock<IUserRepository>();
      var mockUserRoleRepository = new Mock<IUserRoleRepository>();
      var mockRoleRepository = new Mock<IRoleRepository>();
      var mockDateTimeProvider = new Mock<IDateTimeProvider>();
      var mockUnitOfWork = new Mock<IUnitOfWork>();

      mockUserRepository
        .Setup(x => x.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
        .ReturnsAsync((User?)null);

      var handler = CreateHandler(
        mockUserRepository,
        mockUserRoleRepository,
        mockRoleRepository,
        mockDateTimeProvider,
        mockUnitOfWork
      );

      // Act and Assert
      await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));

      mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
    {
      // Arrange
      var user = UserTestFactory.CreateUser();
      user.SetIsDeleted(true, _timestamp);
      var command = new AssignRolesToUserCommand(user.Id, [Ulid.NewUlid()]);

      var mockUserRepository = new Mock<IUserRepository>();
      var mockUserRoleRepository = new Mock<IUserRoleRepository>();
      var mockRoleRepository = new Mock<IRoleRepository>();
      var mockDateTimeProvider = new Mock<IDateTimeProvider>();
      var mockUnitOfWork = new Mock<IUnitOfWork>();

      mockUserRepository
        .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(user);

      var handler = CreateHandler(
        mockUserRepository,
        mockUserRoleRepository,
        mockRoleRepository,
        mockDateTimeProvider,
        mockUnitOfWork
      );

      // Act and Assert
      await Assert.ThrowsAsync<UserAlreadyDeletedException>(() => handler.Handle(command, CancellationToken.None));

      mockRoleRepository.Verify(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/AssignRolesToUser/AssignRolesToUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenAnyRoleDoesNotExist()
    {
      // Arrange
      var user = UserTestFactory.CreateUser();
      var requestedRoleIds = new[] { Ulid.NewUlid(), Ulid.NewUlid() };
      var existingRoleIds = new[] { requestedRoleIds[0] };
      var command = new AssignRolesToUserCommand(user.Id, requestedRoleIds);

      var mockUserRepository = new Mock<IUserRepository>();
      var mockUserRoleRepository = new Mock<IUserRoleRepository>();
      var mockRoleRepository = new Mock<IRoleRepository>();
      var mockDateTimeProvider = new Mock<IDateTimeProvider>();
      var mockUnitOfWork = new Mock<IUnitOfWork>();

      mockUserRepository
        .Setup(x => x.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
        .ReturnsAsync(user);

      mockRoleRepository
        .Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(existingRoleIds);

      var handler = CreateHandler(
        mockUserRepository,
        mockUserRoleRepository,
        mockRoleRepository,
        mockDateTimeProvider,
        mockUnitOfWork
      );

      // Act and Assert
      await Assert.ThrowsAsync<RolesNotFoundException>(() => handler.Handle(command, CancellationToken.None));

      mockUserRoleRepository.Verify(x => x.GetAssignedRoleIdsAsync(It.IsAny<Ulid>(), It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUserRoleRepository.Verify(x => x.AssignRolesToUserAsync(It.IsAny<IReadOnlyCollection<UserRole>>(), It.IsAny<CancellationToken>()), Times.Never);
      mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}