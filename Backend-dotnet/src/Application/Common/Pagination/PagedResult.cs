namespace Application.Common.Pagination;

/// <summary>
/// Represents a paged result.
/// </summary>
/// <typeparam name="T">The type of the items in the paged result.</typeparam>
public sealed class PagedResult<T> where T : class
{
  // Properties
  public required IReadOnlyCollection<T> Items { get; init; }
  public required int PageNumber { get; init; }
  public required int PageSize { get; init; }
  public required int TotalCount { get; init; }

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