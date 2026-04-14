using FluentValidation;

namespace Application.Auth.Commands.ResetPassword;

/// <summary>
/// Represents a validator for the <see cref="ResetPasswordCommand"/> class.
/// </summary>
/// <remarks>FluentValidation is used to validate the command parameters.</remarks>
public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ResetPasswordCommandValidator"/> class.
    /// </summary>
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Reset code is required.")
            .Length(6).WithMessage("Reset code must be 6 characters long.")
            .Must(x => string.IsNullOrEmpty(x) || x.All(char.IsDigit)).WithMessage("Reset code must be numeric.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.");
    }
}