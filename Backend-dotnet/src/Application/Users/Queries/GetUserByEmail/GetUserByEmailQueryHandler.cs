using Application.Common.Abstractions.Persistence;
using Application.Common.Exceptions;
using Application.Users.Dtos;
using Domain.ValueObjects;
using MediatR;

namespace Application.Users.Queries.GetUserByEmail;

/// <summary>
/// Represents a query handler for getting a user by email.
/// </summary>
/// <remarks>Mediator pattern is used to handle the query and return a UserDto.</remarks>
public sealed class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, UserDto?>
{
    // Dependency injection
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _userRoleRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserByEmailQueryHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="userRoleRepository">The user role repository.</param>
    /// <exception cref="ArgumentNullException">Thrown when user repository or role repository is null.</exception>
    public GetUserByEmailQueryHandler(
        IUserRepository userRepository,
        IUserRoleRepository userRoleRepository
    )
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _userRoleRepository = userRoleRepository ?? throw new ArgumentNullException(nameof(userRoleRepository));
    }

    /// <summary>
    /// Handles the query to get a user by email.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns a UserDto representation of the user entity.</returns>
    /// <exception cref="UserHasNoRoleException">Thrown when the user has no roles.</exception>
    public async Task<UserDto?> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);
        var user = await _userRepository.GetByEmailAsync(email, cancellationToken);

        if (user is null)
        {
            return null;
        }

        if (user.IsDeleted)
        {
            return null;
        }
        
        var roles = await _userRoleRepository.GetByUserIdAsync(user.Id, cancellationToken);

        if (!roles.Any())
        {
            throw new UserHasNoRoleException(user.Id);
        }

        return UserDto.FromDomain(user, roles);
    }
}