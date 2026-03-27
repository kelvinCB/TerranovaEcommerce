using Application.Common.Auth;
using Domain.Entities;

namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for creating authentication sessions.
/// </summary>
public interface IAuthSessionService
{
    /// <summary>
    /// Creates an authentication session.
    /// </summary>
    /// <param name="user">The user to create the session for.</param>
    /// <param name="roles">The roles associated with the user.</param>
    /// <returns>Returns the created authentication session.</returns>
    AuthSessionResult Create(User user, IReadOnlyCollection<Role> roles);
}