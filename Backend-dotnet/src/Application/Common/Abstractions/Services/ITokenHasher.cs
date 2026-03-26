namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for hashing and verifying tokens.
/// </summary>
public interface ITokenHasher
{
    /// <summary>
    /// Hashes a token and returns the hashed value.
    /// </summary>
    /// <param name="token">The token to be hashed.</param>
    /// <returns>Returns the hashed token.</returns>
    string Hash(string token);
}