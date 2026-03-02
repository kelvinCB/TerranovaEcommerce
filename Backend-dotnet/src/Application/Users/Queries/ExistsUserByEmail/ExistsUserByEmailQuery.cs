using MediatR;

namespace Application.Users.Queries.ExistsUserByEmail;

/// <summary>
/// Represents a query to check if a user with the specified email exists.
/// </summary>
/// <param name="Email">The email address to check.</param>
/// <remarks>Mediator pattern is used to handle the query and return a boolean value.</remarks>
public sealed record ExistsUserByEmailQuery(string Email) : IRequest<bool>;