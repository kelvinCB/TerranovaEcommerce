using FluentValidation;

namespace Application.Auth.Commands.VerifyEmail;

/// <summary>
/// Validator of <see cref="VerifyEmailCommand" /> class, ensuring that the email and code are provided and valid.
/// </summary>
/// /// <remarks>FluentValidation is used to define validation rules for the command properties.</remarks>
public sealed class VerifyEmailCommandValidator : AbstractValidator<VerifyEmailCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyEmailCommandValidator" /> class.
    /// </summary>
    public VerifyEmailCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required.")
            .Length(6).WithMessage("Code must be 6 characters long.")
            .Must(x => string.IsNullOrEmpty(x) || x.All(char.IsDigit)).WithMessage("Code must contain only digits.");
    }
}