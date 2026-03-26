using Application.Common.Pagination;

namespace Application.Tests.Common.Pagination;

[Trait("Layer", "Application")]
public sealed class PagedResultTests
{
    private sealed class TestItem
    {
        public int Id { get; init; }
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Create")]
    public void Create_ShouldInitialize_WhenParametersAreValid()
    {
        // Arrange
        var items = new[] { new TestItem { Id = 1 }, new TestItem { Id = 2 } };

        // Act
        var result = PagedResult<TestItem>.Create(items, pageNumber: 2, pageSize: 10, totalCount: 25);

        // Assert
        Assert.Equal(items, result.Items);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(25, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Create")]
    public void Create_ShouldThrowException_WhenItemsAreNull()
    {
        // Act and Assert
        Assert.Throws<ArgumentNullException>(() =>
            PagedResult<TestItem>.Create(null!, pageNumber: 1, pageSize: 10, totalCount: 0)
        );
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Create")]
    public void Create_ShouldThrowException_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var items = Array.Empty<TestItem>();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PagedResult<TestItem>.Create(items, pageNumber: 0, pageSize: 10, totalCount: 0)
        );
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Create")]
    public void Create_ShouldThrowException_WhenPageSizeIsLessThanOne()
    {
        // Arrange
        var items = Array.Empty<TestItem>();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PagedResult<TestItem>.Create(items, pageNumber: 1, pageSize: 0, totalCount: 0)
        );
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Create")]
    public void Create_ShouldThrowException_WhenTotalCountIsNegative()
    {
        // Arrange
        var items = Array.Empty<TestItem>();

        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            PagedResult<TestItem>.Create(items, pageNumber: 1, pageSize: 10, totalCount: -1)
        );
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Computed")]
    public void ComputedProperties_ShouldBeCorrect_WhenTotalCountIsZero()
    {
        // Arrange
        var items = Array.Empty<TestItem>();

        // Act
        var result = PagedResult<TestItem>.Create(items, pageNumber: 1, pageSize: 10, totalCount: 0);

        // Assert
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    [Trait("Common", "Pagination/PagedResult/Computed")]
    public void ComputedProperties_ShouldBeCorrect_WhenOnLastPage()
    {
        // Arrange
        var items = new[] { new TestItem { Id = 1 } };

        // Act
        var result = PagedResult<TestItem>.Create(items, pageNumber: 3, pageSize: 10, totalCount: 21);

        // Assert
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }
}
