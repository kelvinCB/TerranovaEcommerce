using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUserById;

/// <summary>
/// Represents a query to get a user by ID.
/// </summary>
/// <param name="Id">The ID of the user to retrieve.</param>
/// <remarks>Mediator pattern is used to handle the query and return a UserDto.</remarks>
public sealed record GetUserByIdQuery(Ulid Id) : IRequest<UserDto?>;