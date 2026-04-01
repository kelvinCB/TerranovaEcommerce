using FluentValidation;

namespace Application.Auth.Commands.Logout;

/// <summary>
/// Validator for the LogoutCommand, ensuring that the provided refresh token is valid and meets the required criteria.
/// </summary>
/// <remarks>FluentValidation pattern is used to validate the command.</remarks>
public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutCommandValidator"/> class and defines the validation rules for the LogoutCommand.
    /// </summary>
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token must be provided.")
            .MaximumLength(512).WithMessage("Refresh token must not exceed 512 characters.");
    }
}