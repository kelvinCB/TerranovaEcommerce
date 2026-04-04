using Application.Auth.Dtos;
using MediatR;

namespace Application.Auth.Commands.RefreshSession;

/// <summary>
/// Represents a command to refresh an authentication session using a refresh token.
/// </summary>
/// <param name="RefreshToken">The refresh token used to refresh the authentication session.</param>
/// <remarks>Mediator pattern is used to handle the command and return AuthSessionDto.</remarks>
public sealed record RefreshSessionCommand(string RefreshToken) : IRequest<AuthSessionDto>;