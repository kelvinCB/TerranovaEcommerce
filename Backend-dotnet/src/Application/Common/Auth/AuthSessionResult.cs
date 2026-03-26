using Domain.Entities;

namespace Application.Common.Auth;

/// <summary>
/// Represents the result of an authentication session.
/// </summary>
public sealed class AuthSessionResult
{
    public required string AccessToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresAt { get; set; }
    public required string RefreshToken { get; set; }
    public required RefreshToken RefreshTokenEntity { get; set; }
}