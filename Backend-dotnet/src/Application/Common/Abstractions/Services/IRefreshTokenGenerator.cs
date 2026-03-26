namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for generating refresh tokens.
/// </summary>
public interface IRefreshTokenGenerator
{
    /// <summary>
    /// Generates a new refresh token.
    /// </summary>
    /// <returns>Returns the generated refresh token.</returns>
    string GenerateRefreshToken();
}