using MediatR;

namespace Application.Auth.Commands.VerifyEmail;

/// <summary>
/// The verify email of a user with email address and verification code.
/// </summary>
/// <param name="Email">The email address of a user.</param>
/// <param name="Code">The verification code to verify email.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record VerifyEmailCommand(
    string Email,
    string Code
) : IRequest<bool>;