using MediatR;

namespace Application.Auth.Commands.Logout;

/// <summary>
/// Represents a command to log out a user by invalidating a refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token to invalidate.</param>
/// <remarks>Mediator pattern is used to handle the command.</remarks>
public sealed record LogoutCommand(string RefreshToken) : IRequest<Unit>;