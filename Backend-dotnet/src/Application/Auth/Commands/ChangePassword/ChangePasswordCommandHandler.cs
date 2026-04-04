using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.ChangePassword;

/// <summary>
/// Represents a handler for the <see cref="ChangePasswordCommand"/> class.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return Unit.</remarks>
public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
{
    // Dependencies
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="refreshTokenRepository">The refresh token repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="dateTimeProvider">The date and time provider.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public ChangePasswordCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IPasswordHasher passwordHasher
    )
    {
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// Handles the change password command.
    /// </summary>
    /// <param name="request">The change password command.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="InvalidCredentialsException">Thrown when the credentials are invalid.</exception>
    public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await GetUserAsync(request.UserId, cancellationToken);
        VerifyPassword(request, user);

        var now = _dateTimeProvider.Timestamp;
        var newPasswordHash = PasswordHash.From(_passwordHasher.HashPassword(request.NewPassword));
        user.SetPasswordHash(newPasswordHash, now);

        await _userRepository.UpdateAsync(user, cancellationToken);
        await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Private methods

    private async Task<User> GetUserAsync(Ulid userId, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

        if (user == null)
        {
            throw new InvalidCredentialsException();
        }

        if (user.IsDeleted)
        {
            throw new InvalidCredentialsException();
        }

        return user;
    }

    private void VerifyPassword(ChangePasswordCommand request, User user)
    {
        var isPasswordValid = _passwordHasher.VerifyPassword(request.CurrentPassword, user.PasswordHash.Value);

        if (!isPasswordValid)
        {
            throw new InvalidCredentialsException();
        }
    }
}