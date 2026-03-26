using Application.Common.Token;

namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for generating JWT tokens.
/// </summary>
public interface IJwtProvider
{
    /// <summary>
    /// Generates a JWT token for the specified user.
    /// </summary>
    /// <param name="request">The user data used to generate the token.</param>
    /// <returns>Returns a JwtTokenResult containing the generated token.</returns>
    JwtTokenResult GenerateToken(JwtUserData request);
}