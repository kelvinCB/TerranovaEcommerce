using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Commands.AssignRolesToUser;

/// <summary>
/// Represents a command handler for assigning roles to a user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class AssignRolesToUserCommandHandler : IRequestHandler<AssignRolesToUserCommand, Unit>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IUserRoleRepository _userRoleRepository;
  private readonly IRoleRepository _roleRepository;
  private readonly IDateTimeProvider _dateTimeProvider;
  private readonly IUnitOfWork _unitOfWork;

  /// <summary>
  /// Initializes a new instance of the <see cref="AssignRolesToUserCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="userRoleRepository">The user role repository.</param>
  /// <param name="roleRepository">The role repository.</param>
  /// <param name="dateTimeProvider">The date-time provider service.</param>
  /// <param name="unitOfWork">The unit of work service.</param>
  /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
  public AssignRolesToUserCommandHandler(
    IUserRepository userRepository,
    IUserRoleRepository userRoleRepository,
    IRoleRepository roleRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
    _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
  }

  /// <summary>
  /// Handles the command to assign roles to a user.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">A cancellation token.</param>
  /// <returns>Returns Unit.Value.</returns>
  /// <exception cref="AtLeastOneRoleMustBeProvidedException">Thrown when no roles are provided.</exception>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  /// <exception cref="UserAlreadyDeletedException">Thrown when the user is already deleted.</exception>
  /// <exception cref="RolesNotFoundException">Thrown when the roles are not found.</exception>
  public async Task<Unit> Handle(AssignRolesToUserCommand request, CancellationToken cancellationToken)
  {
    EnsureAtLeastOneRoleProvided(request.RoleIds);

    var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
    EnsureUserExistsAndNotDeleted(user, request.UserId);

    var validatedRoleIds = await GetValidatedRoleIds(request.RoleIds, cancellationToken);
    var rolesToAssign = await GetRoleIdsToAssignAsync(user!.Id, validatedRoleIds, cancellationToken);

    if (!rolesToAssign.Any())
    {
      return Unit.Value;
    }

    var timestamp = _dateTimeProvider.Timestamp;
    var userRoles = CreateUserRoles(user.Id, rolesToAssign, timestamp);

    await _userRoleRepository.AssignRolesToUserAsync(userRoles, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }

  // Private methods

  private void EnsureAtLeastOneRoleProvided(IReadOnlyCollection<Ulid> roleIds)
  {
    AtLeastOneRoleMustBeProvidedException.ThrowIfNullOrEmpty(roleIds);
  }

  private void EnsureUserExistsAndNotDeleted(User? user, Ulid requestedUserId)
  {
    if (user is null)
    {
      throw new UserNotFoundException(requestedUserId);
    }

    if (user.IsDeleted)
    {
      throw new UserAlreadyDeletedException(requestedUserId);
    }
  }

  private async Task<IReadOnlyCollection<Ulid>> GetValidatedRoleIds(
    IReadOnlyCollection<Ulid> roleIds,
    CancellationToken cancellationToken)
  {
    var requestedRoleIds = roleIds.Distinct().ToArray();

    var existingRoleIds = await _roleRepository.GetExistingRoleIdsAsync(requestedRoleIds, cancellationToken);

    var missingRoleIds = requestedRoleIds.Except(existingRoleIds).ToArray();

    if (missingRoleIds.Any())
    {
      throw new RolesNotFoundException(missingRoleIds);
    }

    return existingRoleIds;
  }

  private async Task<IReadOnlyCollection<Ulid>> GetRoleIdsToAssignAsync(
    Ulid userId,
    IReadOnlyCollection<Ulid> roleIds,
    CancellationToken cancellationToken)
  {
    var assignedRoleIds = await _userRoleRepository.GetAssignedRoleIdsAsync(userId, roleIds, cancellationToken);

    return roleIds.Except(assignedRoleIds).ToArray();
  }

  private IReadOnlyCollection<UserRole> CreateUserRoles(Ulid userId, IReadOnlyCollection<Ulid> roleIds, DateTimeOffset timestamp)
  {
    return roleIds.Select(roleId => UserRole.Create(userId, roleId, timestamp)).ToArray();
  }

}
