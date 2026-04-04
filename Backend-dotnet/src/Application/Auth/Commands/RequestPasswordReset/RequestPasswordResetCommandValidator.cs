using FluentValidation;

namespace Application.Auth.Commands.RequestPasswordReset;

/// <summary>
/// Validator for the RequestPasswordResetCommand, ensuring that the email address is provided and in a valid format.
/// </summary>
/// <remarks>FluentValidation is used to define validation rules for the command.</remarks>
public sealed class RequestPasswordResetCommandValidator : AbstractValidator<RequestPasswordResetCommand>
{
    /// <summary>
    /// Initializes a new instance of <see cref="RequestPasswordResetCommandValidator"/> and defines validation
    /// rules for the <see cref="RequestPasswordResetCommand"/> properties.
    /// </summary>
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(x => x.EmailAddress)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Invalid email address format.");
    }
}