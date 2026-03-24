using Application.Common.Abstractions.Persistence;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Users.Commands.RemoveRolesFromUser;

/// <summary>
/// Represents a command handler for removing roles from a user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class RemoveRolesFromUserCommandHandler : IRequestHandler<RemoveRolesFromUserCommand, Unit>
{
    // Dependency injection
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemoveRolesFromUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userRoleRepository">The user role repository.</param>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public RemoveRolesFromUserCommandHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Handles the command to remove roles from a user.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <exception cref="AtLeastOneRoleMustBeProvidedException">Thrown when no roles are provided.</exception>
    /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
    /// <exception cref="UserAlreadyDeletedException">Thrown when the user is already deleted.</exception>
    /// <exception cref="UserMustHaveAtLeastOneRoleException">Thrown when the user must have at least one role.</exception>
    /// <remarks>Handle will be idempotent and will not throw an exception if the user does not have the specified roles.</remarks>
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

        UserMustHaveAtLeastOneRoleException.ThrowIfNullOrEmpty(remainingRoleIds);

        await _userRoleRepository.RemoveRolesFromUserAsync(user.Id, requestedRoleIds, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}