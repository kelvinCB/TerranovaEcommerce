using FluentValidation;

namespace Application.Auth.Commands.Login;

/// <summary>
/// Validates <see cref="LoginCommand"/>
/// </summary>
/// <remarks>FluentValidation pattern is used to validate the command.</remarks>
public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}