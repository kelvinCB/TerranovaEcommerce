namespace Application.Auth.Commands.Dtos;

/// <summary>
/// Represents a data transfer object (DTO) for an authentication session.
/// </summary>
public sealed class AuthSessionDto
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresAt { get; init; }
    public required DateTimeOffset RefreshTokenExpiresAt { get; init; }
    public required AuthenticatedUserDto User { get; init; }

    /// <summary>
    /// Creates a new instance of the <see cref="AuthSessionDto"/> class.
    /// </summary>
    /// <param name="accessToken">The access token</param>
    /// <param name="accessTokenExpiresAt">The expiration date of the access token</param>
    /// <param name="refreshToken">The refresh token</param>
    /// <param name="refreshTokenExpiresAt">The expiration date of the refresh token</param>
    /// <param name="user">The authenticated user</param>
    /// <returns>Returns a new instance of the <see cref="AuthSessionDto"/> class</returns>
    public static AuthSessionDto Create(
        string accessToken,
        DateTimeOffset accessTokenExpiresAt,
        string refreshToken,
        DateTimeOffset refreshTokenExpiresAt,
        AuthenticatedUserDto user
    ) => new(){
        AccessToken = accessToken,
        AccessTokenExpiresAt = accessTokenExpiresAt,
        RefreshToken = refreshToken,
        RefreshTokenExpiresAt = refreshTokenExpiresAt,
        User = user
    };
}