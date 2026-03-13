using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Users.Commands.SoftDeleteUser;

/// <summary>
/// Represents a command handler for soft deleting a user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class SoftDeleteUserCommandHandler : IRequestHandler<SoftDeleteUserCommand, Unit>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IDateTimeProvider _dateTimeProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="SoftDeleteUserCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="dateTimeProvider">The date-time provider service.</param>
  /// <exception cref="ArgumentNullException">Thrown when the user repository or date-time provider is null.</exception>
  public SoftDeleteUserCommandHandler(
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
  }

  /// <summary>
  /// Handles the command to soft delete a user.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>Returns Unit.Value.</returns>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  /// <exception cref="UserAlreadyDeletedException">Thrown when the user is already deleted.</exception>
  public async Task<Unit> Handle(SoftDeleteUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

    if (user is null)
    {
      throw new UserNotFoundException(request.Id);
    }

    if (user.IsDeleted)
    {
      throw new UserAlreadyDeletedException(request.Id);
    }

    user.SetIsDeleted(true, _dateTimeProvider.Timestamp);

    await _userRepository.SoftDeleteAsync(user, cancellationToken);

    return Unit.Value;
  }
}