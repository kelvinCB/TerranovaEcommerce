using Application.Common.Abstractions.Persistence;
using Application.Common.Pagination;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUsers;

public sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IUserRoleRepository _userRoleRepository;

  public GetUsersQueryHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
  }

  public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
  {
    var users = await _userRepository.GetPagedAsync(request.Page, request.PageSize, request.Search, cancellationToken);

    var userRoleIds = await _userRoleRepository.GetByUserIdAsync(users.Items.Select(x => x.Id).ToList(), cancellationToken);

    var userDtos = users.Items.Select(UserDto.FromDomain).ToList();
  }
}