using Domain.Entities;
using Domain.Tests.Factories;

namespace Domain.Tests.Entities;

[Trait("Layer", "Domain")]
public class RefreshTokenTest
{
  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldCreateRefreshToken_WhenParametersAreValid()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero); // 10 day at the moment of creation
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act
    var refreshToken = RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    );

    // Assert
    Assert.NotNull(refreshToken);
    Assert.Equal(id, refreshToken.Id);
    Assert.Equal(userId, refreshToken.UserId);
    Assert.Equal(tokenHash, refreshToken.TokenHash);
    Assert.Equal(expiresAt, refreshToken.ExpiresAt);
    Assert.Equal(createdAt, refreshToken.CreatedAt);
    Assert.Equal(userAgent, refreshToken.UserAgent);
    Assert.Equal(ipAddress, refreshToken.IpAddress);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenIdIsEmpty()
  {
    // Arrange
    var id = Ulid.Empty; // Invalid id
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero); // 10 day at the moment of creation
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenUserIdIsEmpty()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.Empty; // Invalid userId
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero); // 10 day at the moment of creation
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenTokenHashIsNull()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = default(string); // tokenHash is null
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero); // 10 day at the moment of creation
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash!, // Force non-nullable for testing
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Cannot be null", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenTokenHashIsWhitespace()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "  "; // tokenHash is whitespace
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero); // 10 day at the moment of creation
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Cannot be null or whitespace", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenExpiresAtIsUninitialized()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = default(DateTimeOffset); // Uninitialized
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenExpiresAtIsNotUtc()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenCreatedAtIsUninitialized()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero);
    var createdAt = default(DateTimeOffset);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenCreatedAtIsNotUtc()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero);
    var createdAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Create")]
  public void Create_ShouldThrowException_WhenExpiresAtIsBeforeCreatedAt()
  {
    // Arrange
    var id = Ulid.NewUlid();
    var userId = Ulid.NewUlid();
    var tokenHash = "test-token-hash-001";
    var jwtId = "test-jwt-id-001";
    var expiresAt = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero); // 1 day before
    var createdAt = new DateTimeOffset(2026, 1, 2, 0, 0, 0, TimeSpan.Zero);
    var userAgent = "userAgentTest";
    var ipAddress = "127.0.0.1";

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => RefreshToken.Create(
      id,
      userId,
      tokenHash,
      jwtId,
      expiresAt,
      createdAt,
      userAgent,
      ipAddress
    ));

    Assert.Contains("Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Revoke")]
  public void Revoke_ShouldRevokeToken_WhenTokenIsNotRevokedYet()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var oldIsRevoked = refreshToken.IsRevoked;
    var timestamp = refreshToken.CreatedAt.AddDays(5); // 5 days after creations

    // Act
    refreshToken.Revoke(timestamp);

    // Assert
    Assert.False(oldIsRevoked);
    Assert.True(refreshToken.IsRevoked);
    Assert.Equal(timestamp, refreshToken.RevokedAt);
  }

  [Fact]
  [Trait("RefreshToken", "Revoke")]
  public void Revoke_ShouldThrowException_WhenTokenIsAlreadyRevoked()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    refreshToken.Revoke(refreshToken.CreatedAt.AddDays(1)); // Revoke the token 1 day after creation
    var timestamp = refreshToken.CreatedAt.AddDays(5); // 5 days after creations

    // Act and Assert
    var exception = Assert.Throws<InvalidOperationException>(() => refreshToken.Revoke(timestamp));

    Assert.Contains("The refresh token is already revoked.", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Revoke")]
  public void Revoke_ShouldThrowException_WhenTimestampIsUninitialized()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = default(DateTimeOffset); // Uninitialized

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.Revoke(timestamp));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Revoke")]
  public void Revoke_ShouldThrowException_WhenTimestampIsNotUtc()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken(); // created 2026-01-10 by default in RefreshTokenFactory
    var timestamp = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC.

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.Revoke(timestamp));

    Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "Revoke")]
  public void Revoke_ShouldThrowException_WhenTimestampIsBeforeCreatedAt()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt.AddDays(-1); // 1 day before creation date

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.Revoke(timestamp));

    Assert.Contains($"Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "RevokeByRotation")]
  public void RevokeByRotation_ShouldRevokeToken_WhenTokenIsNotRevokedYet()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var oldIsRevoked = refreshToken.IsRevoked;
    var timestamp = refreshToken.CreatedAt.AddDays(5); // 5 days after creations
    var newTokenId = Ulid.NewUlid();

    // Act
    refreshToken.RevokeByRotation(timestamp, newTokenId);

    // Assert
    Assert.False(oldIsRevoked);
    Assert.True(refreshToken.IsRevoked);
    Assert.Equal(timestamp, refreshToken.RevokedAt);
    Assert.Equal(newTokenId, refreshToken.ReplacedByTokenId);
  }

  [Fact]
  [Trait("RefreshToken", "RevokeByRotation")]
  public void RevokeByRotation_ShouldThrowException_WhenTokenIsAlreadyRevoked()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    refreshToken.RevokeByRotation(refreshToken.CreatedAt, Ulid.NewUlid()); // Revoke the token

    var revokedAt = refreshToken.CreatedAt.AddDays(5); // 5 days after creations
    var newTokenId = Ulid.NewUlid(); // Create a new token that replaced the revoked token

    // Act and Assert
    var exception = Assert.Throws<InvalidOperationException>(() => refreshToken.RevokeByRotation(revokedAt, newTokenId));

    Assert.Contains("The refresh token is already revoked.", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "RevokeByRotation")]
  public void RevokeByRotation_ShouldThrowException_WhenTimestampIsUninitialized()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = default(DateTimeOffset); // Uninitialized
    var newTokenId = Ulid.NewUlid();

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.RevokeByRotation(timestamp, newTokenId));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "RevokeByRotation")]
  public void RevokeByRotation_ShouldThrowException_WhenTimestampIsNotUtc()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken(); // created 2026-01-10 by default in RefreshTokenFactory
    var timestamp = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC.
    var newTokenId = Ulid.NewUlid();

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.RevokeByRotation(timestamp, newTokenId));

    Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "RevokeByRotation")]
  public void RevokeByRotation_ShouldThrowException_WhenTimestampIsBeforeCreatedAt()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt.AddDays(-1); // 1 day before creation date
    var newTokenId = Ulid.NewUlid();

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.RevokeByRotation(timestamp, newTokenId));

    Assert.Contains($"Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "RevokeByRotation")]
  public void RevokeByRotation_ShouldThrowException_WhenNewTokenIdIsEmpty()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt.AddDays(5); // 5 days after creation date
    var newTokenId = Ulid.Empty; // Invalid id

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.RevokeByRotation(timestamp, newTokenId));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "IsExpired")]
  public void IsExpired_ShouldReturnTrue_WhenTokenIsExpired()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.ExpiresAt; // Token is expired

    // Act
    var isExpired = refreshToken.IsExpired(timestamp);

    // Assert
    Assert.True(isExpired);
  }

  [Fact]
  [Trait("RefreshToken", "IsExpired")]
  public void IsExpired_ShouldReturnFalse_WhenTokenIsNotExpired()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.ExpiresAt.AddDays(-1); // 1 day before expiration date

    // Act
    var isExpired = refreshToken.IsExpired(timestamp);

    // Assert
    Assert.False(isExpired);
  }

  [Fact]
  [Trait("RefreshToken", "IsExpired")]
  public void IsExpired_ShouldThrowException_WhenTimestampIsUninitialized()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = default(DateTimeOffset); // Uninitialized

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.IsExpired(timestamp));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "IsExpired")]
  public void IsExpired_ShouldThrowException_WhenTimestampIsNotUtc()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken(); // created 2026-01-10 by default in RefreshTokenFactory
    var timestamp = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC.

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.IsExpired(timestamp));

    Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "IsExpired")]
  public void IsExpired_ShouldThrowException_WhenTimestampIsBeforeCreatedAt()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt.AddDays(-1); // 1 day before creation date

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.IsExpired(timestamp));

    Assert.Contains($"Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "IsExpired")]
  public void IsExpired_ShouldReturnTrue_WhenTimestampIsEqualToExpiresAt()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.ExpiresAt; // Token is expired

    // Act
    var isExpired = refreshToken.IsExpired(timestamp);

    // Assert
    Assert.True(isExpired);
  }

  [Fact]
  [Trait("RefreshToken", "IsActive")]
  public void IsActive_ShouldReturnTrue_WhenTokenIsActive()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt;

    // Act
    var isActive = refreshToken.IsActive(timestamp);

    // Assert
    Assert.True(isActive);
  }

  [Fact]
  [Trait("RefreshToken", "IsActive")]
  public void IsActive_ShouldReturnFalse_WhenTokenIsExpired()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.ExpiresAt; // Token is expired

    // Act
    var isActive = refreshToken.IsActive(timestamp);

    // Assert
    Assert.False(isActive);
  }

  [Fact]
  [Trait("RefreshToken", "IsActive")]
  public void IsActive_ShouldReturnFalse_WhenTokenIsRevoked()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt.AddDays(5); // 5 days after creation date
    refreshToken.Revoke(timestamp);

    // Act
    var isActive = refreshToken.IsActive(timestamp);

    // Assert
    Assert.False(isActive);
  }

  [Fact]
  [Trait("RefreshToken", "IsActive")]
  public void IsActive_ShouldThrowException_WhenTimestampIsUninitialized()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = default(DateTimeOffset); // Uninitialized

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.IsActive(timestamp));

    Assert.Contains("Is uninitialized", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "IsActive")]
  public void IsActive_ShouldThrowException_WhenTimestampIsNotUtc()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken(); // created 2026-01-10 by default in RefreshTokenFactory
    var timestamp = new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.FromHours(-4)); // It's not UTC.

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.IsActive(timestamp));

    Assert.Contains("Must be in UTC", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

  [Fact]
  [Trait("RefreshToken", "IsActive")]
  public void IsActive_ShouldThrowException_WhenTimestampIsBeforeCreatedAt()
  {
    // Arrange
    var refreshToken = RefreshTokenTestFactory.CreateRefreshToken();
    var timestamp = refreshToken.CreatedAt.AddDays(-1); // 1 day before creation date

    // Act and Assert
    var exception = Assert.Throws<ArgumentException>(() => refreshToken.IsActive(timestamp));

    Assert.Contains($"Cannot be before", exception.Message, StringComparison.OrdinalIgnoreCase);
  }

}