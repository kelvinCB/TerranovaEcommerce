using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

/// <summary>
/// Represents a command handler for registering a new user and returning the user ID.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return the user ID.</remarks>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Ulid>
{
    // Dependencies
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIdGenerator _idGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="passwordHasher">The password hasher service.</param>
    /// <param name="dateTimeProvider">The date-time provider service.</param>
    /// <param name="idGenerator">The ID generator service.</param>
    /// <param name="roleRepository">The role repository.</param>
    /// <exception cref="ArgumentNullException">Thrown when the user repository, role repository, password hasher, date-time provider or ID generator is null.</exception>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IIdGenerator idGenerator
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
    }

    /// <summary>
    /// Handles the command to register a new user.
    /// </summary>
    /// <param name="request">The command request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns the user ID.</returns>
    /// <exception cref="AtLeastOneRoleMustBeProvidedException">Thrown when no roles are provided.</exception>
    /// <exception cref="EmailAlreadyInUseException">Thrown when the specified email is already in use.</exception>
    /// <exception cref="RolesNotFoundException">Thrown when the specified roles are not found.</exception>
    public async Task<Ulid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        AtLeastOneRoleMustBeProvidedException.ThrowIfNullOrEmpty(request.RoleIds);

        var email = Email.Create(request.Email);

        var userExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (userExists)
        {
            throw new EmailAlreadyInUseException(email.Value);
        }

        var phoneNumber = default(PhoneNumber);
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            phoneNumber = PhoneNumber.Create(request.PhoneNumber);
        }
        
        var passwordHash = PasswordHash.From(
            _passwordHasher.HashPassword(request.Password)
        );

        var timestamp = _dateTimeProvider.Timestamp;

        var id = _idGenerator.NewUlid();

        var user = User.Create(
            id: id,
            firstName: request.FirstName,
            lastName: request.LastName,
            birthDate: request.BirthDate,
            gender: request.Gender,
            passwordHash: passwordHash,
            timestamp: timestamp,
            emailAddress: email,
            phoneNumber: phoneNumber
        );

        var existingRoles = await _roleRepository.GetExistingRoleIdsAsync(request.RoleIds, cancellationToken);

        var missingRoles = request.RoleIds.Except(existingRoles).ToArray();

        if (missingRoles.Length > 0)
        {
            throw new RolesNotFoundException(missingRoles);
        }
        
        foreach (var roleId in existingRoles)
        {
            user.AssignRole(roleId, timestamp);
        }

        await _userRepository.RegisterAsync(user, cancellationToken);

        return user.Id;
    }
}