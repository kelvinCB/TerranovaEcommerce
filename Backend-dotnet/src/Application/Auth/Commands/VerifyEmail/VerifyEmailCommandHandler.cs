using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.VerifyEmail;

/// <summary>
/// Handles the verification of a user's email address.
/// </summary>
/// <remarks>Mediator pattern is used to handle the command and return a boolean value indicating success or failure.</remarks>
public sealed class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, bool>
{
    // Variables
    private static readonly UserVerificationPurpose userVerificationPurpose = UserVerificationPurpose.EmailVerify;

    // Dependencies
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserVerificationRepository _userVerificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyEmailCommandHandler"/> class with the specified dependencies.
    /// </summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userVerificationRepository">The user verification repository.</param>
    /// <param name="dateTimeProvider">The date and time provider.</param>
    /// <exception cref="ArgumentException">Thrown when any of the dependencies is null.</exception>
    public VerifyEmailCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserVerificationRepository userVerificationRepository,
        IDateTimeProvider dateTimeProvider
    )
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(IUnitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(IUserRepository));
        _userVerificationRepository = userVerificationRepository ?? throw new ArgumentNullException(nameof(IUserVerificationRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(IDateTimeProvider));
    }

    /// <summary>
    /// Handles the verification of a user's email address.
    /// </summary>
    /// <param name="request">The verification request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await GetValidUserAsync(request, cancellationToken);

        if (user is null)
        {
            return false;
        }

        if (user.IsEmailAddressVerified)
        {
            return true;
        }

        var now = _dateTimeProvider.Timestamp;
        var userVerification = await GetValidUserVerificationAsync(user, request.Code, now, cancellationToken);

        if (userVerification is null)
        {
            return false;
        }

        user.SetIsEmailAddressVerified(true, now);
        await _userRepository.UpdateAsync(user, cancellationToken);

        userVerification.Consume(now);
        await _userVerificationRepository.UpdateAsync(userVerification, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }

    // private methods

    private async Task<User?> GetValidUserAsync(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return null;
        }

        return user;
    }

    private async Task<UserVerification?> GetValidUserVerificationAsync(User user, string code, DateTimeOffset timestamp, CancellationToken cancellationToken)
    {
        var normalizedCode = Code.From(code);
        var userVerification = await _userVerificationRepository
            .GetActiveByUserIdPurposeAndCodeAsync(user.Id, userVerificationPurpose, normalizedCode, cancellationToken);
        
        if (
            userVerification is null
            || userVerification.ExpiresAt <= timestamp
            || userVerification.ConsumedAt is not null
            )
        {
            return null;
        }

        return userVerification;
    }
}