using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.RequestPasswordReset;

/// <summary>
/// Handles the request to initiate a password reset process for a user.
/// </summary>
/// <remarks>Mediator Pattern is used to handle the command and return Unit.</remarks>
public sealed class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Unit>
{
    // Variables
    private static readonly UserVerificationPurpose userVerificationPurpose = UserVerificationPurpose.PasswordReset;

    // Dependencies
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserVerificationRepository _userVerificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIdGenerator _idGenerator;
    private readonly INotificationService _notificationService;
    private readonly IVerificationCodeGenerator _verificationCodeGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestPasswordResetCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userVerificationRepository">The user verification repository.</param>
    /// <param name="dateTimeProvider">The date and time provider.</param>
    /// <param name="idGenerator">The ID generator.</param>
    /// <param name="notificationService">The notification service.</param>
    /// <param name="verificationCodeGenerator">The verification code generator.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public RequestPasswordResetCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserVerificationRepository userVerificationRepository,
        IDateTimeProvider dateTimeProvider,
        IIdGenerator idGenerator,
        INotificationService notificationService,
        IVerificationCodeGenerator verificationCodeGenerator)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userVerificationRepository = userVerificationRepository ?? throw new ArgumentNullException(nameof(userVerificationRepository));
        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
        _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));
        _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        _verificationCodeGenerator = verificationCodeGenerator ?? throw new ArgumentNullException(nameof(verificationCodeGenerator));
    }

    /// <summary>
    /// Handles the request to initiate a password reset process for a user.
    /// </summary>
    /// <param name="request">The request password reset command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>Once the user verification is created, it takes 5 minutes to expire.</remarks>
    public async Task<Unit> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var user = await GetValidUserAsync(request, cancellationToken);

        if (user is null)
        {
            return Unit.Value;
        }

        var existsActiveUserVerification = await _userVerificationRepository.ExistsActiveVerificationAsync(
            userId: user.Id,
            purpose: userVerificationPurpose,
            cancellationToken: cancellationToken
        );

        if (existsActiveUserVerification)
        {
            return Unit.Value;
        }

        var generatedVerificationCode = _verificationCodeGenerator.Generate();

        var userVerification = CreateUserVerification(user, generatedVerificationCode.Code);

        await _userVerificationRepository.AddAsync(userVerification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendPasswordResetCodeAsync(user.EmailAddress.Value, generatedVerificationCode.PlainTextCode, cancellationToken);

        return Unit.Value;
    }

    // Private methods

    private async Task<User?> GetValidUserAsync(RequestPasswordResetCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.EmailAddress);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return null;
        }

        return user;
    }

    private UserVerification CreateUserVerification(User user, Code code)
    {
        var verificationId = _idGenerator.NewUlid();
        var verificationCode = code;
        var now = _dateTimeProvider.Timestamp;

        var userVerification = UserVerification.Create(
            id: verificationId,
            userId: user.Id,
            purpose: userVerificationPurpose,
            verificationCode: verificationCode,
            expiresAt: now.AddMinutes(5),
            createdAt: now
        );

        return userVerification;
    }

    private async Task SendPasswordResetCodeAsync(string email, string codePlainText, CancellationToken cancellationToken)
    {
        await _notificationService.SendPasswordResetCodeToEmailAsync(email, codePlainText, cancellationToken);
    }
}