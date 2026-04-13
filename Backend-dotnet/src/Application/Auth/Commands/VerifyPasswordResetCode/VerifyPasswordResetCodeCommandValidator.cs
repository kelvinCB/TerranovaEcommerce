using FluentValidation;

namespace Application.Auth.Commands.VerifyPasswordResetCode;

/// <summary>
/// Validator for the VerifyPasswordResetCodeCommand, ensuring that the email and code are provided and valid.
/// </summary>
/// <remarks>FluentValidation is used to define validation rules for the command properties.</remarks>
public sealed class VerifyPasswordResetCodeCommandValidator : AbstractValidator<VerifyPasswordResetCodeCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="VerifyPasswordResetCodeCommandValidator"/> class.
    /// </summary>
    public VerifyPasswordResetCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required.")
            .Length(6).WithMessage("Code must be 6 characters long.")
            .Must(x => string.IsNullOrEmpty(x) || x.All(char.IsDigit)).WithMessage("Code must contain only digits.");
    }
}