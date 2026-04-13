using MediatR;

namespace Application.Auth.Commands.VerifyPasswordResetCode;

/// <summary>
/// Represents a command to verify a password reset code for a user.
/// </summary>
/// <param name="Email">The email of the user.</param>
/// <param name="Code">The password reset code.</param>
/// <remarks>Mediator pattern is used to handle the command and return a boolean value.</remarks>
public sealed record VerifyPasswordResetCodeCommand(string Email, string Code) : IRequest<bool>;