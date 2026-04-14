using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.ResetPassword;

/// <summary>
/// Handles the reset password command, allowing users to reset their password using a verification code.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return Unit.</remarks>
public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
{
    // Variables
    private static readonly UserVerificationPurpose userVerificationPurpose = UserVerificationPurpose.PasswordReset;

    // Dependencies
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserVerificationRepository _userVerificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandHandler"/> class.
    /// </summary>
    /// <param name="refreshTokenRepository">The refresh token repository.</param>
    /// <param name="unitOfWork">The unit of work.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userVerificationRepository">The user verification repository.</param>
    /// <param name="dateTimeProvider">The date time provider.</param>
    /// <param name="passwordHasher">The password hasher.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public ResetPasswordCommandHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserVerificationRepository userVerificationRepository,
        IDateTimeProvider dateTimeProvider,
        IPasswordHasher passwordHasher
    )
    {
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userVerificationRepository = userVerificationRepository ?? throw new ArgumentNullException(nameof(userVerificationRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// Handles the reset password command by validating the verification code,
    /// updating the user's password hash, and consuming the verification code.
    /// </summary>
    /// <param name="request">The reset password command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await GetValidUserOrNullAsync(request, cancellationToken);

        if (user is null)
        {
            return Unit.Value;
        }

        var now = _dateTimeProvider.Timestamp;

        var userVerification = await GetValidUserVerificationAsync(user.Id, request.Code, now, cancellationToken);

        var newPasswordHash = GetValidNewPasswordHash(request, user);
        user.SetPasswordHash(newPasswordHash, now);

        userVerification.Consume(now);

        await _userVerificationRepository.UpdateAsync(userVerification, cancellationToken);
        await _userRepository.UpdateAsync(user, cancellationToken);
        await _refreshTokenRepository.RevokeAllForUserAsync(user.Id, now, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    // Private methods

    private async Task<User?> GetValidUserOrNullAsync(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        return user is not null && !user.IsDeleted ? user : null;
    }

    private async Task<UserVerification> GetValidUserVerificationAsync(Ulid userId, string code, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var userVerification = await _userVerificationRepository
            .GetActiveByUserIdAndPurposeAsync(userId, userVerificationPurpose, cancellationToken);

        if (
            userVerification is null
            || !userVerification.VerificationCode.IsMatch(code)
            || userVerification.ConsumedAt is not null
            || userVerification.ExpiresAt <= now
        )
        {
            throw new InvalidOrExpiredVerificationCodeException();
        }
        
        return userVerification;
    }

    private PasswordHash GetValidNewPasswordHash(ResetPasswordCommand request, User user)
    {
        bool isSamePassword = _passwordHasher.VerifyPassword(request.NewPassword, user.PasswordHash.Value);

        if (isSamePassword)
        {
            throw new NewPasswordSameAsOldPasswordException();
        }

        var newPasswordHash = PasswordHash.From(_passwordHasher.HashPassword(request.NewPassword));
        return newPasswordHash;
    }
}