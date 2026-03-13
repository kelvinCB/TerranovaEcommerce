using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Users.Commands.SetUserActivationStatus;

/// <summary>
/// Represents a command handler for changing the activation status of a user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class SetUserActivationStatusCommandHandler : IRequestHandler<SetUserActivationStatusCommand, Unit>
{
  // Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IDateTimeProvider _dateTimeProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="SetUserActivationStatusCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="dateTimeProvider">The date-time provider service.</param>
  /// <exception cref="ArgumentNullException">Thrown when the user repository or date-time provider is null.</exception>
  public SetUserActivationStatusCommandHandler(
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider)
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
  }

  public async Task<Unit> Handle(SetUserActivationStatusCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

    if (user is null)
    {
      throw new UserNotFoundException(request.Id);
    }

    if(user.IsActive == request.IsActive)
    {
      throw new UserActivationStatusNotChangedException(request.Id);
    }

    user.SetIsActive(request.IsActive, _dateTimeProvider.Timestamp);

    await _userRepository.SetIsActiveAsync(user, cancellationToken);

    return Unit.Value;
  }
}