using MediatR;
using Application.Common.Exceptions;
using Application.Common.Abstractions.Persistence;

namespace Application.Users.Commands.RemoveRolesFromUser;

/// <summary>
/// Represents a command handler for removing roles from a user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class RemoveRolesFromUserCommandHandler : IRequestHandler<RemoveRolesFromUserCommand, Unit>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IUserRoleRepository _userRoleRepository;

  /// <summary>
  /// Initializes a new instance of the <see cref="RemoveRolesFromUserCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="userRoleRepository">The user role repository.</param>
  /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
  public RemoveRolesFromUserCommandHandler(IUserRepository userRepository, IUserRoleRepository userRoleRepository)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
  }

  /// <summary>
  /// Handles the command to remove roles from a user.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">A cancellation token.</param>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  /// <exception cref="UserAlreadyDeletedException">Thrown when the user is already deleted.</exception>
  /// <exception cref="UserMustHaveAtLeastOneRoleException">Thrown when the user must have at least one role.</exception>
  public async Task<Unit> Handle(RemoveRolesFromUserCommand request, CancellationToken cancellationToken)
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

    var assignedRoleIds = await _userRoleRepository.GetRoleIdsByUserIdAsync(user.Id, cancellationToken);

    var remainingRoleIds = assignedRoleIds.Except(requestedRoleIds).ToArray();

    if (remainingRoleIds.Length == 0)
    {
      throw new UserMustHaveAtLeastOneRoleException();
    }

    await _userRoleRepository.RemoveRolesFromUserAsync(user.Id, requestedRoleIds, cancellationToken);

    return Unit.Value;
  }
}