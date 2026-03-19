using Application.Common.Pagination;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUsers;

/// <summary>
/// Represents a query to get a list of users.
/// </summary>
/// <param name="Page">The page number to retrieve</param>
/// <param name="PageSize">The number of items per page</param>
/// <param name="Search">The search query</param>
/// <remarks>Mediator pattern is used to handle the query and return a PagedResult of UserDto.</remarks>
public sealed record GetUsersQuery(
  int Page,
  int PageSize,
  string? Search
) : IRequest<PagedResult<UserDto>>;