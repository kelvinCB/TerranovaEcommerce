using Application.Auth.Dtos;
using MediatR;

namespace Application.Auth.Commands.Login;

/// <summary>
/// Login command
/// </summary>
/// <param name="EmailAddress">The user's email address</param>
/// <param name="Password">The user's password</param>
/// <remarks>Mediator pattern is used to handle the command and return AuthSessionDto.</remarks>
public sealed record LoginCommand(
    string EmailAddress,
    string Password
) : IRequest<AuthSessionDto>;