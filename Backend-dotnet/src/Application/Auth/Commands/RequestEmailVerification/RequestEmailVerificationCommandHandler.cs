using Application.Common.Abstractions.Persistence;
using Application.Common.Abstractions.Services;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;

namespace Application.Auth.Commands.RequestEmailVerification;

/// <summary>
/// Handles the request to request email verification for a user.
/// </summary>
/// <remarks>Mediator Pattern is used to handle the command and return Unit.</remarks>
public sealed class RequestEmailVerificationCommandHandler : IRequestHandler<RequestEmailVerificationCommand, Unit>
{
    // Variables
    private static readonly UserVerificationPurpose userVerificationPurpose = UserVerificationPurpose.EmailVerify;
    
    // Dependencies
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserVerificationRepository _userVerificationRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IIdGenerator _idGenerator;
    private readonly INotificationService _notificationService;
    private readonly IVerificationCodeGenerator _verificationCodeGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEmailVerificationCommandHandler"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work service.</param>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userVerificationRepository">The user verification repository.</param>
    /// <param name="dateTimeProvider">The date and time provider.</param>
    /// <param name="idGenerator">The ID generator.</param>
    /// <param name="notificationService">The notification service.</param>
    /// <param name="verificationCodeGenerator">The verification code generator.</param>
    /// <exception cref="ArgumentNullException">Thrown when any of the dependencies is null.</exception>
    public RequestEmailVerificationCommandHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserVerificationRepository userVerificationRepository,
        IDateTimeProvider dateTimeProvider,
        IIdGenerator idGenerator,
        INotificationService notificationService,
        IVerificationCodeGenerator verificationCodeGenerator
    )
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
    /// Handles requests to send an email verification code.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <remarks>
    /// Once the user verification is created, it takes 5 minutes to expire.
    /// If the user does not exist, is deleted, or already has an active email verification,
    /// the command will complete without any action.
    /// </remarks>
    public async Task<Unit> Handle(RequestEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var user = await GetValidUserAsync(request, cancellationToken);

        if (user is null)
        {
            return Unit.Value;
        }

        if (user.IsEmailAddressVerified)
        {
            return Unit.Value;
        }

        var existsActiveVerification = await VerifyExistingActiveVerificationAsync(user.Id, cancellationToken);

        if (existsActiveVerification)
        {
            return Unit.Value;
        }
        
        var generatedCode = _verificationCodeGenerator.Generate();

        var userVerification = CreateUserVerification(user, generatedCode.Code);

        await _userVerificationRepository.AddAsync(userVerification, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await SendEmailVerificationCodeAsync(user.EmailAddress.Value, generatedCode.PlainTextCode, cancellationToken);

        return Unit.Value;
    }

    // Private methods

    private async Task<User?> GetValidUserAsync(RequestEmailVerificationCommand request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null || user.IsDeleted)
        {
            return null;
        }

        return user;
    }

    private async Task<bool> VerifyExistingActiveVerificationAsync(Ulid userId, CancellationToken cancellationToken)
    {
        var existsActiveVerification = await _userVerificationRepository
            .ExistsActiveVerificationAsync(userId, userVerificationPurpose, cancellationToken);

        return existsActiveVerification;
    }

    private UserVerification CreateUserVerification(User user, Code code)
    {
        var verificationId = _idGenerator.NewUlid();
        var verificationCode = code;
        var now = _dateTimeProvider.Timestamp;

        return UserVerification.Create(
            id: verificationId,
            userId: user.Id,
            purpose: userVerificationPurpose,
            verificationCode: verificationCode,
            expiresAt: now.AddMinutes(5),
            createdAt: now
        );
    }

    private async Task SendEmailVerificationCodeAsync(string email, string codePlainText, CancellationToken cancellationToken)
    {
        await _notificationService.SendEmailVerificationCodeToEmailAsync(email, codePlainText, cancellationToken);
    }
}