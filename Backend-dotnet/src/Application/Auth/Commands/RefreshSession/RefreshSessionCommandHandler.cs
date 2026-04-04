using Application.Auth.Dtos;
using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using MediatR;

namespace Application.Auth.Commands.RefreshSession;

/// <summary>
/// Handles the refresh session command.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return AuthSessionDto.</remarks>
public sealed class RefreshSessionCommandHandler : IRequestHandler<RefreshSessionCommand, AuthSessionDto>
{
    // Dependencies
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuthSessionService _authSessionService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITokenHashService _tokenHashService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshSessionCommandHandler"/> class.
    /// </summary>
    /// <param name="refreshTokenRepository">The refresh token repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userRoleRepository">The user role repository.</param>
    /// <param name="authSessionService">The authentication session service.</param>
    /// <param name="dateTimeProvider">The date time provider.</param>
    /// <param name="tokenHashService">The token hash service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public RefreshSessionCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository,
        IAuthSessionService authSessionService,
        IDateTimeProvider dateTimeProvider,
        ITokenHashService tokenHashService)
    {
        _tokenHashService = tokenHashService ?? throw new ArgumentNullException(nameof(tokenHashService));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
        _authSessionService = authSessionService ?? throw new ArgumentNullException(nameof(authSessionService));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
    }

    /// <summary>
    /// Handles the refresh session command.
    /// </summary>
    /// <param name="request">The refresh token command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The authentication session DTO.</returns>
    /// <exception cref="InvalidCredentialsException">Thrown when the credentials are invalid.</exception>
    /// <exception cref="RefreshTokenNotActiveException">Thrown when the refresh token is not active.</exception>
    /// <exception cref="UserMustHaveAtLeastOneRoleException">Thrown when the user associated with the refresh token does not have any roles.</exception>
    public async Task<AuthSessionDto> Handle(RefreshSessionCommand request, CancellationToken cancellationToken)
    {
        var hashedToken = _tokenHashService.HashToken(request.RefreshToken);
        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(hashedToken, cancellationToken);

        if (refreshToken is null)
        {
            throw new InvalidCredentialsException();
        }

        var now = _dateTimeProvider.Timestamp;

        if (!refreshToken.IsActive(now))
        {
            throw new RefreshTokenNotActiveException();
        }

        var user = await GetUserAsync(refreshToken.UserId, cancellationToken);

        var roles = await GetRolesAsync(user.Id, cancellationToken);

        var session = _authSessionService.Create(user, roles);

        refreshToken.RevokeByRotation(now, session.RefreshTokenEntity.Id);

        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);
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

    private async Task<User> GetUserAsync(Ulid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

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

    private async Task<IReadOnlyCollection<Role>> GetRolesAsync(Ulid userId, CancellationToken cancellationToken)
    {
        var roles = await _userRoleRepository.GetByUserIdAsync(userId, cancellationToken);

        UserMustHaveAtLeastOneRoleException.ThrowIfNullOrEmpty(roles.Select(x => x.Id).ToArray());

        return roles;
    }
}