using FluentValidation;

namespace Application.Auth.Commands.RefreshSession;

/// <summary>
/// Validator for the RefreshSessionCommand, ensuring that the provided refresh token is valid and meets the required criteria.
/// </summary>
/// <remarks>FluentValidation is used to define validation rules for the RefreshSessionCommand.</remarks>
public sealed class RefreshSessionCommandValidator : AbstractValidator<RefreshSessionCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshSessionCommandValidator"/> class.
    /// </summary>
    public RefreshSessionCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required.")
            .MaximumLength(512).WithMessage("Refresh token must not exceed 512 characters.");
    }
}