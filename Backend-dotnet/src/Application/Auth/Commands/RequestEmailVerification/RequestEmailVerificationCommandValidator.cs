using FluentValidation;

namespace Application.Auth.Commands.RequestEmailVerification;

/// <summary>
/// Validator of <see cref="RequestEmailVerificationCommand" /> class, ensuring that the email is provided and valid.
/// </summary>
/// <remarks>FluentValidation is used to define validation rules for the command properties.</remarks>
public sealed class RequestEmailVerificationCommandValidator : AbstractValidator<RequestEmailVerificationCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequestEmailVerificationCommandValidator" /> class.
    /// </summary>
    public RequestEmailVerificationCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}