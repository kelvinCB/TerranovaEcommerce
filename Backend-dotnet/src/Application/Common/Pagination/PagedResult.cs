namespace Application.Common.Pagination;

/// <summary>
/// Represents a paged result.
/// </summary>
/// <typeparam name="T">The type of the items in the paged result.</typeparam>
public sealed class PagedResult<T>
{
    // Properties
    public IReadOnlyCollection<T> Items { get; private set; }
    public int PageNumber { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items in the paged result.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total count.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the page number or page size is less than 1, or the total count is less than 0.</exception>
    private PagedResult(
        IReadOnlyCollection<T> items,
        int pageNumber,
        int pageSize,
        int totalCount
    )
    {
        ArgumentNullException.ThrowIfNull(items);

        if (pageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageNumber));
        }

        if (pageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(pageSize));
        }

        if (totalCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalCount));
        }

        Items = items;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    /// <param name="items">The items in the paged result.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="totalCount">The total count.</param>
    /// <returns>Returns a new instance of the <see cref="PagedResult{T}"/> class.</returns>
    public static PagedResult<T> Create(
        IReadOnlyCollection<T> items,
        int pageNumber,
        int pageSize,
        int totalCount
    )
    {
        return new PagedResult<T>(
            items: items,
            pageNumber: pageNumber,
            pageSize: pageSize,
            totalCount: totalCount
        );
    }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Indicates whether the current page has a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates whether the current page has a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}