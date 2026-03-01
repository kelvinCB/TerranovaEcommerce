using Application.Common.Abstractions.Persistence;
using Application.Users.Dtos;
using MediatR;

namespace Application.Users.Queries.GetUserById;

/// <summary>
/// Represents a query handler for getting a user by ID.
/// </summary>
/// <remarks>Mediator pattern is used to handle the query and return a UserDto.</remarks>
public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto?>
{
    // Dependency injection
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Handles the query to get a user by ID.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns a UserDto representation of the user entity.</returns>
    public async Task<UserDto?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if(user is null)
        {
            return null;
        }

        return UserDto.FromDomain(user);
    }
    
}