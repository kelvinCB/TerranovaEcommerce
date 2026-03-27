using Application.Auth.Dtos;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.Login;

/// <summary>
/// Handles the login command.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return AuthSessionDto.</remarks>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthSessionDto>
{
    // Dependencies
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthSessionService _authSessionService;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginCommandHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userRoleRepository">The user role repository.</param>
    /// <param name="refreshTokenRepository">The refresh token repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="authSessionService">The auth session service.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null</exception>
    public LoginCommandHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IAuthSessionService authSessionService,
        IPasswordHasher passwordHasher
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _authSessionService = authSessionService ?? throw new ArgumentNullException(nameof(authSessionService));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// Handles the login command.
    /// </summary>
    /// <param name="request">The login command</param>
    /// <param name="cancellationToken">A cancellation token</param>
    /// <exception cref="InvalidCredentialsException">Thrown when the credentials are invalid</exception>
    /// <returns>Returns an AuthSessionDto</returns>
    public async Task<AuthSessionDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(request, cancellationToken);
        VerifyPassword(request.Password, user.PasswordHash.Value);

        var roles = await GetRolesAsync(user.Id, cancellationToken);

        var session = _authSessionService.Create(user, roles);

        await _refreshTokenRepository.AddAsync(session.RefreshTokenEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return AuthSessionDto.Create(
            accessToken: session.AccessToken,
            accessTokenExpiresAt: session.AccessTokenExpiresAt,
            refreshToken: session.RefreshToken,
            refreshTokenExpiresAt: session.RefreshTokenEntity.ExpiresAt,
            user: AuthenticatedUserDto.FromDomain(user, roles)
        );
    }

    // Private methods
    
    private async Task<User> GetUserAsync(LoginCommand request, CancellationToken cancellationToken)
    {
        var emailAddress = Email.Create(request.EmailAddress);
        var user = await _userRepository.GetByEmailAsync(emailAddress, cancellationToken);

        if (user is null)
        {
            throw new InvalidCredentialsException();
        }

        if (user.IsDeleted)
        {
            throw new InvalidCredentialsException();
        }

        return user;
    }

    private void VerifyPassword(string requestedPassword, string hashedPassword)
    {
        bool isValid = _passwordHasher.VerifyPassword(requestedPassword, hashedPassword);
        if (!isValid)
        {
            throw new InvalidCredentialsException();
        }
    }

    private async Task<IReadOnlyCollection<Role>> GetRolesAsync(Ulid userId, CancellationToken cancellationToken)
    {
        var roles = await _userRoleRepository.GetByUserIdAsync(userId, cancellationToken);

        AtLeastOneRoleMustBeProvidedException.ThrowIfNullOrEmpty(roles.Select(x => x.Id).ToArray());

        return roles;
    }
}