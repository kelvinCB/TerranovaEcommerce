using Domain.Entities;

namespace Domain.Tests.Factories;

public static class RefreshTokenTestFactory
{
  public static RefreshToken CreateRefreshToken()
  {
    return RefreshToken.Create(
      id: Ulid.NewUlid(),
      userId: Ulid.NewUlid(),
      tokenHash: "test-token-hash-001",
      jwtId: "test-jwt-id-001",
      expiresAt: new DateTimeOffset(2026, 1, 10, 0, 0, 0, TimeSpan.Zero),
      createdAt: new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
      userAgent: "userAgentTest",
      ipAddress: "127.0.0.1"
    );
  }
}