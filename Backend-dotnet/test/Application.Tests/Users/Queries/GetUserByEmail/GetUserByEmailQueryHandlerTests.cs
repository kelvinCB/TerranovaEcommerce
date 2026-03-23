using Application.Users.Queries.GetUserByEmail;
using Application.Common.Abstractions.Persistence;
using Domain.Entities;
using Domain.ValueObjects;
using Moq;
using Application.Common.Exceptions;
using Common.Tests.Factories;

namespace Application.Tests.Users.Queries.GetUserByEmail;

[Trait("Layer", "Application")]
public sealed class GetUserByEmailQueryHandlerTests
{
    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidRepository()
    {
        // Act and Assert
        var exception = Record.Exception(() =>
            new GetUserByEmailQueryHandler(
                Mock.Of<IUserRepository>(),
                Mock.Of<IUserRoleRepository>()
            )
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetUserByEmailQueryHandler(
                default!, // Force non-nullable for testing
                Mock.Of<IUserRoleRepository>()
            )
        );
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserRoleRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetUserByEmailQueryHandler(
                Mock.Of<IUserRepository>(),
                default! // Force non-nullable for testing
            )
        );
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Handle")]
    public async Task Handle_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();

        var role = new List<Role>();

        role.Add(RoleTestFactory.CreateRole());

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(role);

        var handler = new GetUserByEmailQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByEmailQuery(user.EmailAddress.Value), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByEmailAsync(user.EmailAddress, CancellationToken.None), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), CancellationToken.None), Times.AtLeastOnce);

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
        Assert.NotEmpty(result.Roles);
        Assert.True(role.All(x => result.Roles.Select(y => y.Id).Contains(x.Id)));
        Assert.True(role.All(x => result.Roles.Select(x => x.Name).Contains(x.Name)));
        Assert.True(role.All(x => result.Roles.Select(x => x.Description).Contains(x.Description)));
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Handle")]
    public async Task Handle_ShouldReturnNull_WhenUserDoesNotExist()
    {
        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var handler = new GetUserByEmailQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByEmailQuery("test@example.com"), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.Null(result);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Handle")]
    public async Task Handle_ShouldReturnNull_WhenUserIsDeleted()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();

        user.SetIsDeleted(true, new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)); // Deleted 1 Day from creation date

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var handler = new GetUserByEmailQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act
        var result = await handler.Handle(new GetUserByEmailQuery(user.EmailAddress.Value), CancellationToken.None);

        // Assert
        mockUserRepository.Verify(x => x.GetByEmailAsync(user.EmailAddress, CancellationToken.None), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.Null(result);
    }

    [Fact]
    [Trait("Users", "Queries/GetUserByEmailQueryHandler/Handle")]
    public async Task Handle_ShouldThrowException_WhenUserHasNoRoles()
    {
        // Arrange
        var user = UserTestFactory.CreateUser();

        var mockUserRepository = new Mock<IUserRepository>();
        var mockUserRoleRepository = new Mock<IUserRoleRepository>();

        mockUserRepository
            .Setup(x => x.GetByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        mockUserRoleRepository
            .Setup(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Role>());

        var handler = new GetUserByEmailQueryHandler(mockUserRepository.Object, mockUserRoleRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<UserHasNoRoleException>(async () => await handler.Handle(new GetUserByEmailQuery(user.EmailAddress.Value), CancellationToken.None));

        mockUserRepository.Verify(x => x.GetByEmailAsync(user.EmailAddress, CancellationToken.None), Times.Once);
        mockUserRoleRepository.Verify(x => x.GetByUserIdAsync(It.IsAny<Ulid>(), CancellationToken.None), Times.AtLeastOnce);
    }
}