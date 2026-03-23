using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

/// <summary>
/// Represents a command handler for updating a user profile.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
  /// Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IDateTimeProvider _dateTimeProvider;
  private readonly IUnitOfWork _unitOfWork;

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="dateTimeProvider">The date-time provider service.</param>
  /// <param name="unitOfWork">The unit of work service.</param>
  /// <exception cref="ArgumentNullException">Thrown when the user repository or date-time provider is null.</exception>
  public UpdateUserCommandHandler(
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider,
    IUnitOfWork unitOfWork
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
  }

  /// <summary>
  /// Handles the command to update a user profile.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>Returns Unit.Value.</returns>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
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

    var timestamp = _dateTimeProvider.Timestamp;

    if (request.BirthDate.HasValue)
      user.SetBirthDate(request.BirthDate.Value, timestamp);

    user.Update(
      timestamp: timestamp,
      firstName: request.FirstName,
      lastName: request.LastName,
      gender: request.Gender
    );

    await _userRepository.UpdateAsync(user, cancellationToken);
    await _unitOfWork.SaveChangesAsync(cancellationToken);

    return Unit.Value;
  }
}