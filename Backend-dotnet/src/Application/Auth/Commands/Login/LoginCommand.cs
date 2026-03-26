using Application.Auth.Commands.Dtos;
using MediatR;

namespace Application.Auth.Commands.Login;

/// <summary>
/// Login command
/// </summary>
/// <param name="EmailAddress">The user's email address</param>
/// <param name="Password">The user's password</param>
/// <marks>Mediator pattern is used to handle the command and return AuthSessionDto.</marks>
public sealed record LoginCommand(
    string EmailAddress,
    string Password
) : IRequest<AuthSessionDto>;