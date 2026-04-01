namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for hashing tokens.
/// </summary>
public interface ITokenHashService
{
    /// <summary>
    /// Hashes the provided token and returns the hashed value.
    /// </summary>
    /// <param name="token">The token to hash.</param>
    /// <returns>The hashed token.</returns>
    string HashToken(string token);
}