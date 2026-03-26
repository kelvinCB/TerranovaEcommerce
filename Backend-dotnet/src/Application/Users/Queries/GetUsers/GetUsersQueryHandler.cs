using Application.Common.Pagination;
using Application.Common.ReadModels.Users.Models;
using Application.Common.ReadModels.Persistence;
using MediatR;

namespace Application.Users.Queries.GetUsers;

/// <summary>
/// Represents a query handler for getting a list of users and their roles as a paged result.
/// </summary>
/// <remarks>Mediator pattern is used to handle the query and return a PagedResult of UserListItem.</remarks>
public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserListItem>>
{
    // Dependency injection
    private readonly IUserReadModelRepository _userReadModelRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUsersQueryHandler"/> class.
    /// </summary>
    /// <param name="userReadModelRepository">The user read model repository.</param>
    public GetUsersQueryHandler(
        IUserReadModelRepository userReadModelRepository
    )
    {
        _userReadModelRepository = userReadModelRepository ?? throw new ArgumentNullException(nameof(userReadModelRepository));
    }
    public async Task<PagedResult<UserListItem>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var pagedUsers = await _userReadModelRepository.GetPagedAsync(request.Page, request.PageSize, request.Search, cancellationToken);

        return pagedUsers;
    }
}