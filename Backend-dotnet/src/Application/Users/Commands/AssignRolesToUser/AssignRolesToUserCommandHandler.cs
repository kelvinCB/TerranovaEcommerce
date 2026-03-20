using Application.Common.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Users.Commands.AssignRolesToUser;

/// <summary>
/// Represents a command handler for assigning roles to a user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public record class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, Unit>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IUserRoleRepository _userRoleRepository;
  private readonly IRoleRepository _roleRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="AssignRolesToUserCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="userRoleRepository">The user role repository.</param>
  /// <param name="roleRepository">The role repository.</param>
  /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
  public AssignRolesToUserCommandHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
  }

  /// <summary>
  /// Handles the command to assign roles to a user.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">A cancellation token.</param>
  /// <returns>Returns Unit.Value.</returns>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  /// <exception cref="UserAlreadyDeletedException">Thrown when the user is already deleted.</exception>
  /// <exception cref="RolesNotFoundException">Thrown when the roles are not found.</exception>
  public async Task<Unit> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
  {
    AtLeastOneRoleMustBeProvidedException.ThrowIfNullOrEmpty(request.RoleIds);

    var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

    if (user is null)
    {
      throw new UserNotFoundException(request.UserId);
    }

    if (user.IsDeleted)
    {
      throw new UserAlreadyDeletedException(request.UserId);
    }

    var requestedRoleIds = request.RoleIds.Distinct().ToArray();

    var existingRoleIds = await _roleRepository.GetExistingRoleIdsAsync(requestedRoleIds, cancellationToken);

    var missingRoleIds = requestedRoleIds.Except(existingRoleIds).ToArray();

    if (missingRoleIds.Any())
    {
      throw new RolesNotFoundException(missingRoleIds);
    }

    var assignedRoleIds = await _userRoleRepository.GetAssignedRoleIdsAsync(user.Id, requestedRoleIds, cancellationToken);

    var rolesToAssign = requestedRoleIds.Except(assignedRoleIds).ToArray();

    if (rolesToAssign.Any())
    {
      await _userRoleRepository.AssignRolesToUserAsync(user.Id, rolesToAssign, cancellationToken);
    }

    return Unit.Value;
  }
}