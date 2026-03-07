using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Commands.UpdateUser;

/// <summary>
/// Represents a command for updating a user profile.
/// </summary>
/// <param name="Id">The ID of the user to update.</param>
/// <param name="FirstName">The first name of the user.</param>
/// <param name="LastName">The last name of the user.</param>
/// <param name="Gender">The gender of the user.</param>
/// <param name="BirthDate">The birth date of the user.</param>
/// <remarks>Mediator pattern is used to handle the command and return the updated user profile.</remarks>
public sealed record UpdateUserCommand(
  Ulid Id,
  string? FirstName,
  string? LastName,
  char? Gender,
  DateOnly? BirthDate
) : IRequest<UserDto>;