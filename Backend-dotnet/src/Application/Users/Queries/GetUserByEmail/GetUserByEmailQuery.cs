using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUserByEmail;

/// <summary>
/// Represents a query to get a user by email.
/// </summary>
/// <param name="Email">The email address of the user to retrieve.</param>
/// <remarks>Mediator pattern is used to handle the query and return a UserDto.</remarks>
public sealed record GetUserByEmailQuery(string Email) : IRequest<UserDto?>;