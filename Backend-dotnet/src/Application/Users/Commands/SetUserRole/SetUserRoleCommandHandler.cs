using MediatR;
using Application.Common.Exceptions;
using Application.Common.Abstractions.Services;
using Application.Common.Abstractions.Persistence;
using Domain.Entities;

namespace Application.Users.Commands.SetUserRole;

/// <summary>
/// Represents a command handler for setting a user's role.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class SetUserRoleCommandHandler : IRequestHandler<SetUserRoleCommand, Unit>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IRoleRepository _roleRepository;
  private readonly IUserRoleRepository _userRoleRepository;
  private readonly IDateTimeProvider _dateTimeProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="SetUserRoleCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="roleRepository">The role repository.</param>
  /// <param name="userRoleRepository">The user role repository.</param>
  /// <param name="dateTimeProvider">The date-time provider service.</param>
  /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
  public SetUserRoleCommandHandler(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IUserRoleRepository userRoleRepository,
    IDateTimeProvider dateTimeProvider
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
  }

  /// <summary>
  /// Handles the command to set a user's role.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">A cancellation token.</param>
  /// <returns>Returns Unit.Value.</returns>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  /// <exception cref="UserAlreadyDeletedException">Thrown when the user is already deleted.</exception>
  /// <exception cref="RoleNotFoundException">Thrown when the role is not found.</exception>
  /// <exception cref="UserAlreadyHasRoleException">Thrown when the user already has the role</exception>
  public async Task<Unit> Handle(SetUserRoleCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

    if (user is null)
    {
      throw new UserNotFoundException(request.UserId);
    }

    if (user.IsDeleted)
    {
      throw new UserAlreadyDeletedException(request.UserId);
    }

    var roleExists = await _roleRepository.ExistsByIdAsync(request.RoleId, cancellationToken);

    if (!roleExists)
    {
      throw new RoleNotFoundException(request.RoleId);
    }

    var existsUserRole = await _userRoleRepository.ExistsUserRoleAsync(user.Id, request.RoleId, cancellationToken);

    if (existsUserRole)
    {
      throw new UserAlreadyHasRoleException(user.Id, request.RoleId);
    }

    var userRole = UserRole.Create(user.Id, request.RoleId, _dateTimeProvider.Timestamp);

    await _userRoleRepository.RegisterAsync(userRole, cancellationToken);

    return Unit.Value;
  }
}