using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Application.Users.Dtos;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

/// <summary>
/// Represents a command handler for updating a user profile.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return the updated user profile.</remarks>
public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserDto>
{
  /// Dependencies
  private readonly IUserRepository _userRepository;
  private readonly IDateTimeProvider _dateTimeProvider;

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdateUserCommandHandler"/> class.
  /// </summary>
  /// <param name="userRepository">The user repository.</param>
  /// <param name="dateTimeProvider">The date-time provider service.</param>
  /// <exception cref="ArgumentNullException">Thrown when the user repository is null.</exception>
  public UpdateUserCommandHandler(
    IUserRepository userRepository,
    IDateTimeProvider dateTimeProvider
  )
  {
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
  }

  /// <summary>
  /// Handles the command to update a user profile.
  /// </summary>
  /// <param name="request">The command request.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>Returns the updated user profile.</returns>
  /// <exception cref="UserNotFoundException">Thrown when the user is not found.</exception>
  public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

    if (user is null)
    {
      throw new UserNotFoundException(request.Id);
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

    return UserDto.FromDomain(user);
  }
}