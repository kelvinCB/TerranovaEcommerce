using Application.Users.Queries.GetUserById;
using Application.Common.Abstractions.Persistence;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;
using Application.Common.Exceptions;
using Common.Tests.Factories;

namespace Application.Tests.Users.Queries.GetUserById;

[Trait("Layer", "Application")]
public sealed class GetUserByIdQueryHandlerTests
{
    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidRepository()
    {
        // Act and Assert
        var exception = Record.Exception(() => 
            new GetUserByIdQueryHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetUserByIdQueryHandler(
                default!, // Force non-nullable for testing
                Mock.Of<IUserRoleRepository>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenRoleRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => 
            new GetUserByIdQueryHandler(
                Mock.Of<IUserRepository>(),
                default! // Force non-nullable for testing
            )
        );
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Handle")]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange

        var user = UserTestFactory.CreateUser();

        var role = new List<Role>();

        role.Add(RoleTestFactory.CreateRole());

        role.ForEach(x => user.AssignRole(x.Id, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)));

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = new GetUserByIdQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByIdAsync(user.Id, CancellationToken.None), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);

        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.FirstName, result.FirstName);
        Assert.Equal(user.LastName, result.LastName);
        Assert.Equal(user.BirthDate, result.BirthDate);
        Assert.Equal(user.Gender, result.Gender);
        Assert.Equal(user.IsActive, result.IsActive);
        Assert.Equal(user.CreatedAt, result.CreatedAt);
        Assert.Equal(user.UpdatedAt, result.UpdatedAt);
        Assert.Equal(user.EmailAddress.Value, result.EmailAddress);
        Assert.Equal(user.IsDeleted, result.IsDeleted);
        Assert.Equal(user.PhoneNumber?.Value, result.PhoneNumber);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Handle")]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByIdQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByIdQuery(Ulid.NewUlid()), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.Null(result);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserIsDeleted()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();

        user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)); // Deleted 1 Day from creation date

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserByIdQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act & Assert
        var result = await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None);

        mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.Null(result);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByIdQueryHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserHasNoRoles()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role>());

        var handler = new GetUserByIdQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UserHasNoRoleException>(async () => await handler.Handle(new GetUserByIdQuery(user.Id), CancellationToken.None));

        mockUserRepository.Verify(x => x.GetByIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }

}