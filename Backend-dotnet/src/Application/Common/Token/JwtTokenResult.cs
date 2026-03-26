namespace Application.Common.Token;

/// <summary>
/// Represents the result of a JWT token operation.
/// </summary>
public sealed class JwtTokenResult
{
    public required string Token { get; init; }
    public required string JwtId { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}