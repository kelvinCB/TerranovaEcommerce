using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.VerifyPasswordResetCode;

/// <summary>
/// Handles the verification of a password reset code for a user. It checks if the provided email and code are valid and not expired.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return a boolean value.</remarks>
public sealed class VerifyPasswordResetCodeCommandHandler : IRequestHandler<VerifyPasswordResetCodeCommand, bool>
{
    // Dependencies
    private readonly IUserRepository _userRepository;
    private readonly IUserVerificationRepository _userVerificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyPasswordResetCodeCommandHandler"/> class with the specified dependencies.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userVerificationRepository">The user verification repository.</param>
    /// <param name="dateTimeProvider">The date time provider.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public VerifyPasswordResetCodeCommandHandler(
        IUserRepository userRepository,
        IUserVerificationRepository userVerificationRepository,
        IDateTimeProvider dateTimeProvider
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userVerificationRepository = userVerificationRepository ?? throw new ArgumentNullException(nameof(userVerificationRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    /// <summary>
    /// Handles the verification of the password reset code. It checks if the user exists, if the verification code matches, and if it is not expired.
    /// </summary>
    /// <param name="request">The verification command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A boolean indicating whether the code is valid.</returns>
    public async Task<bool> Handle(VerifyPasswordResetCodeCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return false;
        }

        var code = Code.From(request.Code);
        var userVerification = await _userVerificationRepository.GetActiveByUserIdPurposeAndCodeAsync(user.Id, UserVerificationPurpose.PasswordReset, code, cancellationToken);
        var now = _dateTimeProvider.Timestamp;

        if (
            userVerification is null 
            || userVerification.VerificationCode.IsMatch(request.Code) is false
            || userVerification.ExpiresAt <= now 
            || userVerification.ConsumedAt is not null)
        {
            return false;
        }

        return true;
    }
}