using Application.Common.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Users.Commands.RemoveRolesFromUser;
using Common.Tests.Factories;
using Domain.Entities;
using MediatR;
using Moq;

namespace Application.Tests.Users.Commands.RemoveRolesFromUser;

[Trait("Layer", "Application")]
public sealed class RemoveRolesFromUserCommandHandlerTests
{
    private readonly DateTimeOffset _timestamp = new(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);

    private static RemoveRolesFromUserCommandHandler CreateHandler(
        Mock<IUserRepository> userRepository,
        Mock<IUserRoleRepository> userRoleRepository,
        Mock<IUnitOfWork> unitOfWork)
    {
        return new RemoveRolesFromUserCommandHandler(
            userRepository.Object,
            userRoleRepository.Object,
            unitOfWork.Object
        );
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        // Act
        var exception = Record.Exception(() =>
            new RemoveRolesFromUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IUnitOfWork>()
            )
        );

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RemoveRolesFromUserCommandHandler(
                default!,
                Mock.Of<IUserRoleRepository>(),
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RemoveRolesFromUserCommandHandler(
                Mock.Of<IUserRepository>(),
                default!,
                Mock.Of<IUnitOfWork>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUnitOfWorkIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            new RemoveRolesFromUserCommandHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>(),
                default!
            )
        );
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Handle")]
    public async Task Handle_ShouldRemoveRolesAndSaveChanges_WhenRequestIsValid()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();
        var assignedRoleIds = new[] { Ulid.NewUlid(), Ulid.NewUlid(), Ulid.NewUlid() };
        var requestedRoleIds = new[] { assignedRoleIds[0], assignedRoleIds[1], assignedRoleIds[1] };
        var command = new RemoveRolesFromUserCommand(user.Id, requestedRoleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(user);

        mockUserRoleRepository
            .Setup(x => x.GetRoleIdsByUserIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(assignedRoleIds);

        var handler = CreateHandler(mockUserRepository, mockUserRoleRepository, mockUnitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        mockUserRoleRepository.Verify(x => x.RemoveRolesFromUserAsync(
                user.Id,
                It.Is<IReadOnlyCollection<Ulid>>(roleIds =>
                    roleIds.Count == 2 &&
                    roleIds.Contains(assignedRoleIds[0]) &&
                    roleIds.Contains(assignedRoleIds[1])),
                CancellationToken.None),
            Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenNoRolesAreProvided()
    {
        // Arrange
        var command = new RemoveRolesFromUserCommand(Ulid.NewUlid(), []);
        var handler = CreateHandler(
            new Mock<IUserRepository>(),
            new Mock<IUserRoleRepository>(),
            new Mock<IUnitOfWork>()
        );

        // Act & Assert
        await Assert.ThrowsAsync<AtLeastOneRoleMustBeProvidedException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserDoesNotExist()
    {
        // Arrange
        var command = new RemoveRolesFromUserCommand(Ulid.NewUlid(), [Ulid.NewUlid()]);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(command.UserId, CancellationToken.None))
            .ReturnsAsync((User?)null);

        var handler = CreateHandler(mockUserRepository, mockUserRoleRepository, mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRoleRepository.Verify(x => x.GetRoleIdsByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.RemoveRolesFromUserAsync(It.IsAny<Ulid>(), It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();
        user.SetIsDeleted(true, _timestamp);
        var command = new RemoveRolesFromUserCommand(user.Id, [Ulid.NewUlid()]);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(user);

        var handler = CreateHandler(mockUserRepository, mockUserRoleRepository, mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<UserAlreadyDeletedException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRoleRepository.Verify(x => x.GetRoleIdsByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUserRoleRepository.Verify(x => x.RemoveRolesFromUserAsync(It.IsAny<Ulid>(), It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserWouldHaveNoRolesRemaining()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();
        var assignedRoleIds = new[] { Ulid.NewUlid(), Ulid.NewUlid() };
        var command = new RemoveRolesFromUserCommand(user.Id, assignedRoleIds);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(user);
        mockUserRoleRepository
            .Setup(x => x.GetRoleIdsByUserIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(assignedRoleIds);

        var handler = CreateHandler(mockUserRepository, mockUserRoleRepository, mockUnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<UserMustHaveAtLeastOneRoleException>(() => handler.Handle(command, CancellationToken.None));

        mockUserRoleRepository.Verify(x => x.RemoveRolesFromUserAsync(It.IsAny<Ulid>(), It.IsAny<IReadOnlyCollection<Ulid>>(), It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    [Trait("Users", "Commands/RemoveRolesFromUser/RemoveRolesFromUserCommandHandler/Handle")]
    public async Task Handle_ShouldIgnoreUnassignedRoles_WhenAtLeastOneRoleRemains()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();
        var assignedRoleIds = new[] { Ulid.NewUlid(), Ulid.NewUlid() };
        var unassignedRoleId = Ulid.NewUlid();
        var command = new RemoveRolesFromUserCommand(user.Id, [assignedRoleIds[0], unassignedRoleId]);

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(user);

        mockUserRoleRepository
            .Setup(x => x.GetRoleIdsByUserIdAsync(user.Id, CancellationToken.None))
            .ReturnsAsync(assignedRoleIds);

        var handler = CreateHandler(mockUserRepository, mockUserRoleRepository, mockUnitOfWork);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(Unit.Value, result);
        mockUserRoleRepository.Verify(x => x.RemoveRolesFromUserAsync(
                user.Id,
                It.Is<IReadOnlyCollection<Ulid>>(roleIds =>
                    roleIds.Count == 2 &&
                    roleIds.Contains(assignedRoleIds[0]) &&
                    roleIds.Contains(unassignedRoleId)),
                CancellationToken.None),
            Times.Once);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }
}