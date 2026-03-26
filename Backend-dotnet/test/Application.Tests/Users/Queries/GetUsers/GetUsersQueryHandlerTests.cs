using Application.Common.Pagination;
using Application.Common.ReadModels.Persistence;
using Application.Common.ReadModels.Users.Models;
using Application.Users.Queries.GetUsers;
using Moq;

namespace Application.Tests.Users.Queries.GetUsers;

[Trait("Layer", "Application")]
public sealed class GetUsersQueryHandlerTests
{
    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryHandler/Constructor")]
    public void Constructor_ShouldInitialize_WithValidDependencies()
    {
        // Act and Assert
        var exception = Record.Exception(() =>
            new GetUsersQueryHandler(Mock.Of<IUserReadModelRepository>())
        );

        Assert.Null(exception);
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryHandler/Constructor")]
    public void Constructor_ShouldThrowException_WhenUserReadModelRepositoryIsNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() =>
            new GetUsersQueryHandler(default!)
        );
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryHandler/Handle")]
    public async Task Handle_ShouldReturnPagedUsers_WhenQueryIsValid()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 2, PageSize: 10, Search: "brian");

        var items = new[]
        {
            new UserListItem
            {
                Id = Ulid.NewUlid(),
                FirstName = "Briangel",
                LastName = "Santana",
                EmailAddress = "test@example.com",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Roles = Array.Empty<RoleListItem>()
            }
        };

        var expected = PagedResult<UserListItem>.Create(items, pageNumber: 2, pageSize: 10, totalCount: 21);

        var repository = new Mock<IUserReadModelRepository>();
        repository
            .Setup(x => x.GetPagedAsync(query.Page, query.PageSize, query.Search, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var handler = new GetUsersQueryHandler(repository.Object);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        repository.Verify(x => x.GetPagedAsync(query.Page, query.PageSize, query.Search, CancellationToken.None), Times.Once);
        Assert.Same(expected, result);
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryHandler/Handle")]
    public async Task Handle_ShouldPropagate_CancellationToken()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        var query = new GetUsersQuery(Page: 1, PageSize: 10, Search: null);

        var expected = PagedResult<UserListItem>.Create(
            Array.Empty<UserListItem>(),
            pageNumber: 1,
            pageSize: 10,
            totalCount: 0);

        var repository = new Mock<IUserReadModelRepository>();
        repository
            .Setup(x => x.GetPagedAsync(query.Page, query.PageSize, query.Search, token))
            .ReturnsAsync(expected);

        var handler = new GetUsersQueryHandler(repository.Object);

        // Act
        await handler.Handle(query, token);

        // Assert
        repository.Verify(x => x.GetPagedAsync(query.Page, query.PageSize, query.Search, token), Times.Once);
    }
}
