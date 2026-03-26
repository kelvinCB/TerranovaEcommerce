using Application.Users.Queries.GetUsers;

namespace Application.Tests.Users.Queries.GetUsers;

[Trait("Layer", "Application")]
public sealed class GetUsersQueryValidatorTests
{
    private readonly GetUsersQueryValidator _validator = new();

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldPass_WhenQueryIsValid()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 1, PageSize: 10, Search: "brian");

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldFail_WhenPageIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 0, PageSize: 10, Search: null);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetUsersQuery.Page));
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldFail_WhenPageSizeIsLessThanOrEqualToZero()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 1, PageSize: 0, Search: null);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetUsersQuery.PageSize));
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldFail_WhenPageSizeIsGreaterThanOneHundred()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 1, PageSize: 101, Search: null);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetUsersQuery.PageSize));
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldFail_WhenSearchLengthIsGreaterThanOneHundred()
    {
        // Arrange
        var search = new string('a', 101);
        var query = new GetUsersQuery(Page: 1, PageSize: 10, Search: search);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetUsersQuery.Search));
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldPass_WhenSearchIsNull()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 1, PageSize: 10, Search: null);

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Users", "Queries/GetUsers/GetUsersQueryValidator/Validate")]
    public void Validate_ShouldPass_WhenSearchIsWhiteSpace()
    {
        // Arrange
        var query = new GetUsersQuery(Page: 1, PageSize: 10, Search: "   ");

        // Act
        var result = _validator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
    }
}
