using FluentValidation;

namespace Application.Auth.Commands.ChangePassword;

/// <summary>
/// Represents a validator for the <see cref="ChangePasswordCommand"/> class.
/// </summary>
/// <remarks>FluentValidation is used to validate the command parameters.</remarks>
public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChangePasswordCommandValidator"/> class.
    /// </summary>
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");
    }
}