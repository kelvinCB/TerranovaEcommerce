using FluentValidation;

namespace Application.Auth.Commands.Logout;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Token must be provided.")
            .MaximumLength(512).WithMessage("Token must not exceed 512 characters.");
    }
}