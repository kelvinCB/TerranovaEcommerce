using Application.Common.Abstractions.Persistence;
using Domain.ValueObjects;
using MediatR;

namespace Application.Users.Queries.ExistsUserByEmail;

/// <summary>
/// Represents a query handler for checking if a user with the specified email exists.
/// </summary>
/// <remarks>Mediator pattern is used to handle the query and return a boolean value.</remarks>
public sealed class ExistsUserByEmailQueryHandler : IRequestHandler<ExistsUserByEmailQuery, bool>
{
    // Dependency injection
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExistsUserByEmailQueryHandler"/> class.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <exception cref="ArgumentNullException">Thrown when the user repository is null.</exception>
    public ExistsUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <summary>
    /// Handles the query to check if a user with the specified email exists.
    /// </summary>
    /// <param name="request">The query request.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Returns a boolean value indicating whether a user with the specified email exists.</returns>
    public async Task<bool> Handle(ExistsUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var email = Email.Create(request.Email);

        bool exists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);

        return exists;
    }
}