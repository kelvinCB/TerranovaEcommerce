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
    // Dependency injection
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIdGenerator _idGenerator;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="roleRepository">The role repository.</param>
    /// <param name="userRoleRepository">The user role repository.</param>
    /// <param name="passwordHasher">The password hasher service.</param>
    /// <param name="dateTimeProvider">The date-time provider service.</param>
    /// <param name="idGenerator">The ID generator service.</param>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider,
        IIdGenerator idGenerator,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
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
        EnsureRolesProvided(request.RoleIds);

        var email = Email.Create(request.Email);

        await EnsureEmailIsAvailableAsync(email, cancellationToken);

        var phoneNumber = CreatePhoneNumber(request.PhoneNumber);
        var passwordHash = CreatePasswordHash(request.Password);
        var timestamp = _dateTimeProvider.Timestamp;
        var userId = _idGenerator.NewUlid();

        var user = CreateUser(request, userId, passwordHash, email, phoneNumber, timestamp);

        var existingRoles = await GetValidatedRolesAsync(request.RoleIds, cancellationToken);
        var userRoles = CreateUserRoles(userId, existingRoles, timestamp);

        await _userRepository.RegisterAsync(user, cancellationToken);
        await _userRoleRepository.AssignRolesToUserAsync(userRoles, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    // Private methods
    
    private void EnsureRolesProvided(IReadOnlyCollection<Ulid> roleIds)
    {
        AtLeastOneRoleMustBeProvidedException.ThrowIfNullOrEmpty(roleIds);
    }

    private async Task EnsureEmailIsAvailableAsync(Email email, CancellationToken cancellationToken)
    {
        var userExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (userExists)
        {
            throw new EmailAlreadyInUseException(email.Value);
        }
    }

    private PhoneNumber? CreatePhoneNumber(string? phoneNumber)
    {
        return string.IsNullOrWhiteSpace(phoneNumber) ? default : PhoneNumber.Create(phoneNumber);
    }

    private PasswordHash CreatePasswordHash(string password)
    {
        return PasswordHash.From(_passwordHasher.HashPassword(password));
    }

    private User CreateUser(RegisterUserCommand request, Ulid userId, PasswordHash passwordHash, Email email, PhoneNumber? phoneNumber, DateTimeOffset timestamp)
    {
        return User.Create(
            id: userId,
            firstName: request.FirstName,
            lastName: request.LastName,
            birthDate: request.BirthDate,
            gender: request.Gender,
            passwordHash: passwordHash,
            timestamp: timestamp,
            emailAddress: email,
            phoneNumber: phoneNumber
        );
    }

    private async Task<IReadOnlyCollection<Ulid>> GetValidatedRolesAsync(IReadOnlyCollection<Ulid> roleIds, CancellationToken cancellationToken)
    {
        var requestedRoleIds = roleIds.Distinct().ToArray();

        var existingRoles = await _roleRepository.GetExistingRoleIdsAsync(requestedRoleIds, cancellationToken);

        var missingRoles = requestedRoleIds.Except(existingRoles).ToArray();

        if (missingRoles.Length > 0)
        {
            throw new RolesNotFoundException(missingRoles);
        }

        return existingRoles;
    }

    private IReadOnlyCollection<UserRole> CreateUserRoles(Ulid userId, IReadOnlyCollection<Ulid> roleIds, DateTimeOffset timestamp)
    {
        var userRoles = roleIds.Select(roleId => UserRole.Create(userId, roleId, timestamp)).ToArray();

        return userRoles;
    }
}