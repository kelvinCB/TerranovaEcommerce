using MediatR;

namespace Application.Auth.Commands.RequestPasswordReset;

/// <summary>
/// Request a password reset for a user by their email address.
/// </summary>
/// <param name="EmailAddress">The email address of the user requesting a password reset.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record RequestPasswordResetCommand(string EmailAddress) : IRequest<Unit>;