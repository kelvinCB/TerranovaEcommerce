using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using MediatR;

namespace Application.Auth.Commands.Logout;

/// <summary>
/// Handles the logout command.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
{
    // Dependencies
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITokenHashService _tokenHashService;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandHandler"/> class.
    /// </summary>
    /// <param name="refreshTokenRepository">The refresh token repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="dateTimeProvider">The date time provider.</param>
    /// <param name="tokenHashService">The token hash service.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public LogoutCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ITokenHashService tokenHashService)
    {
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _tokenHashService = tokenHashService ?? throw new ArgumentNullException(nameof(tokenHashService));
    }

    /// <summary>
    /// Handles the logout command.
    /// </summary>
    /// <param name="request">The logout command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="RefreshTokenNotFoundException">Thrown when the refresh token is not found.</exception>
    public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = _tokenHashService.HashToken(request.RefreshToken);

        var refreshToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash, cancellationToken);

        if (refreshToken is null)
        {
            throw new RefreshTokenNotFoundException();
        }

        var now = _dateTimeProvider.Timestamp;

        refreshToken.Revoke(now);

        await _refreshTokenRepository.UpdateAsync(refreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}