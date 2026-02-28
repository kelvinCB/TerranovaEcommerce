using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tests.Factories;

public static class UserVerificationTestFactory
{
    public static UserVerification CreateUserVerification()
    {
        return UserVerification.Create(
            id: Ulid.NewUlid(),
            userId: Ulid.NewUlid(),
            purpose: "email_verify",
            verificationCode: Code.From("123456"),
            expiresAt: new DateTimeOffset(2022, 1, 1, 0, 5, 0, TimeSpan.Zero),
            createdAt: new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );
    }
}