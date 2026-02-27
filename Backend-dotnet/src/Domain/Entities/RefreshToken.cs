using Domain.Validations;

namespace Domain.Entities;

/// <summary>
/// Represents a refresh token associated with a user
/// </summary>
public class RefreshToken
{
  // Properties
  public Ulid Id { get; private set; }
  public Ulid UserId { get; private set; }
  public string TokenHash { get; private set; } = null!;
  public string? JwtId { get; private set; }
  public DateTimeOffset ExpiresAt { get; private set; }
  public bool IsRevoked { get; private set; }
  public DateTimeOffset? RevokedAt { get; private set; }
  public Ulid? ReplacedByTokenId { get; private set; }
  public DateTimeOffset CreatedAt { get; private set; }
  public string? UserAgent { get; private set; }
  public string? IpAddress { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="RefreshToken"/> class with the specified parameters 
  /// </summary>
  /// <param name="id">The refresh token identifier</param>
  /// <param name="userId">The user identifier</param>
  /// <param name="tokenHash">The hash of the refresh token</param>
  /// <param name="jwtId">The JWT identifier</param>
  /// <param name="expiresAt">The expiration date of the refresh token</param>
  /// <param name="createdAt">The timestamp when the refresh token was created</param>
  /// <param name="userAgent">The user agent of the user at the time of creation</param>
  /// <param name="ipAddress">The IP Address of the user at the time of creation</param>
  private RefreshToken(
    Ulid id,
    Ulid userId,
    string tokenHash,
    string? jwtId,
    DateTimeOffset expiresAt,
    DateTimeOffset createdAt,
    string? userAgent,
    string? ipAddress
  )
  {
    Id = id;
    UserId = userId;
    TokenHash = tokenHash;
    JwtId = jwtId;
    ExpiresAt = expiresAt;
    CreatedAt = createdAt;
    UserAgent = userAgent;
    IpAddress = ipAddress;
  }

  /// <summary>
  /// Creates a new instance of the <see cref="RefreshToken"/> class with specified parameters and performs necessary validations 
  /// </summary>
  /// <param name="id">The refresh token identifier</param>
  /// <param name="userId">The user identifier</param>
  /// <param name="tokenHash">The token hash</param>
  /// <param name="jwtId">The JWT identifier</param>
  /// <param name="expiresAt">The expiration date</param>
  /// <param name="createdAt">The creation date</param>
  /// <param name="userAgent">The user agent of the user at the time of creation</param>
  /// <param name="ipAddress">The IP Address of the user at the time of creation</param>
  /// <returns>Returns a new instance of the <see cref="RefreshToken"/> class</returns>
  public static RefreshToken Create(
    Ulid id,
    Ulid userId,
    string tokenHash,
    string? jwtId,
    DateTimeOffset expiresAt,
    DateTimeOffset createdAt,
    string? userAgent,
    string? ipAddress
  )
  {
    // Perform validations using the Guard class to ensure domain invariants are maintained
    Guard.EnsureUlidNotEmpty(id, nameof(id));
    Guard.EnsureUlidNotEmpty(userId, nameof(userId));
    Guard.EnsureStringNotNullOrWhiteSpace(tokenHash, nameof(tokenHash));
    Guard.EnsureUtc(expiresAt, nameof(expiresAt));
    Guard.EnsureUtc(createdAt, nameof(createdAt));
    Guard.EnsureUtcNotBefore(expiresAt, createdAt, nameof(expiresAt));

    return new RefreshToken(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    );
  }

  /// <summary>
  /// Revokes the refresh token by updating the IsRevoked and RevokedAt properties
  /// </summary>
  /// <param name="timestamp">The timestamp when the refresh token was revoked</param>
  /// <exception cref="InvalidOperationException">Thrown when the refresh token is already revoked</exception>
  public void Revoke(DateTimeOffset timestamp)
  {
    if (IsRevoked)
      throw new InvalidOperationException("The refresh token is already revoked.");

    // Perform validations using the Guard class to ensure domain invariants are maintained
    Guard.EnsureUtc(timestamp, nameof(timestamp));
    Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

    IsRevoked = true;
    RevokedAt = timestamp;
  }

  /// <summary>
  /// Revokes the refresh token by updating the IsRevoked, RevokedAt and ReplacedByTokenId properties
  /// </summary>
  /// <param name="timestamp">The timestamp when the refresh token was revoked</param>
  /// <param name="newTokenId">The new refresh token identifier to replace the revoked token</param>
  /// <exception cref="InvalidOperationException">Thrown when the refresh token is already revoked</exception>
  public void RevokeByRotation(DateTimeOffset timestamp, Ulid newTokenId)
  {
    if (IsRevoked)
      throw new InvalidOperationException("The refresh token is already revoked.");

    // Perform validations using the Guard class to ensure domain invariants are maintained
    Guard.EnsureUtc(timestamp, nameof(timestamp));
    Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));
    Guard.EnsureUlidNotEmpty(newTokenId, nameof(newTokenId));

    IsRevoked = true;
    RevokedAt = timestamp;
    ReplacedByTokenId = newTokenId;
  }

  /// <summary>
  /// Checks if the refresh token is expired based on the provided timestamp
  /// </summary>
  /// <param name="timestamp">The timestamp to check if the refresh token is expired</param>
  /// <returns>Returns true if the refresh token is expired, false otherwise</returns>
  public bool IsExpired(DateTimeOffset timestamp)
  {
    // Perform validations using the Guard class to ensure domain invariants are maintained
    Guard.EnsureUtc(timestamp, nameof(timestamp));
    Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

    bool isExpired = timestamp >= ExpiresAt;

    return isExpired;
  }

  /// <summary>
  /// Checks if the refresh token is active based on the provided timestamp
  /// </summary>
  /// <param name="timestamp">The timestamp to check if the refresh token is active</param>
  /// <returns>Returns true if the refresh token is active, false otherwise</returns>
  public bool IsActive(DateTimeOffset timestamp)
  {
    // Perform validations using the Guard class to ensure domain invariants are maintained
    Guard.EnsureUtc(timestamp, nameof(timestamp));
    Guard.EnsureUtcNotBefore(timestamp, CreatedAt, nameof(timestamp));

    bool isActive = !IsRevoked && !IsExpired(timestamp);

    return isActive;
  }
  
}