namespace Application.Common.Abstractions.Services;

/// <summary>
/// Represents a service for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>Returns the hashed password.</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hashed password.
    /// </summary>
    /// <param name="password">The password to verify.</param>
    /// <param name="hashedPassword">The hashed password to compare against.</param>
    /// <returns>Returns true if the password matches the hashed password, false otherwise.</returns>
    bool VerifyPassword(string password, string hashedPassword);
}