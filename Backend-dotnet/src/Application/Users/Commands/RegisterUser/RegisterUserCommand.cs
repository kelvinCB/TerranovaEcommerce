using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

/// <summary>
/// Represents a command for registering a new user.
/// </summary>
/// <param name="FirstName">The first name of the user</param>
/// <param name="LastName">The last name of the user</param>
/// <param name="PhoneNumber">The phone number of the user</param>
/// <param name="BirthDate">The birth date of the user</param>
/// <param name="Gender">The gender of the user</param>
/// <param name="Password">The password of the user</param>
/// <param name="Email">The email address of the user</param>
/// <remarks>Mediator pattern is used to handle the command and return the registered user.</remarks>
public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    DateOnly BirthDate,
    char Gender,
    string Password,
    string Email
) : IRequest<UserDto>;