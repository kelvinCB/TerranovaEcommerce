using MediatR;

namespace Application.Auth.Commands.RequestEmailVerification;

/// <summary>
/// Represents a command to request email verification for a user.
/// </summary>
/// <param name="Email">The email address for which to request verification.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record RequestEmailVerificationCommand(string Email) : IRequest<Unit>;