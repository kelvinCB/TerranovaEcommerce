using Application.Users.Dtos;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

/// <summary>
/// Represents a command handler for registering a new user and returning the registered user.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return the registered user.</remarks>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserDto>
{
    // Dependencies
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="passwordHasher">The password hasher service.</param>
    /// <param name="dateTimeProvider">The date time provider service.</param>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <summary>
    /// Handles the command to register a new user.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the registered user.</returns>
    /// <exception cref="EmailAlreadyInUseException">Thrown when the specified email is already in use.</exception>
    public async Task<UserDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        var phoneNumber = default(PhoneNumber);
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        }

        var userExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (userExists)
        {
            throw new EmailAlreadyInUseException(email.Value);
        }
        
        var passwordHash = PasswordHash.From(
            _passwordHasher.HashPassword(request.Password)
        );

        var timestamp = _dateTimeProvider.Timestamp;

        var user = User.Create(
            id: Ulid.NewUlid(),
            firstName: request.FirstName,
            lastName: request.LastName,
            birthDate: request.BirthDate,
            gender: request.Gender,
            passwordHash: passwordHash,
            timestamp: timestamp,
            emailAddress: email,
            phoneNumber: phoneNumber
        );

        await _userRepository.RegisterAsync(user, cancellationToken);

        return UserDto.FromDomain(user);
    }
}