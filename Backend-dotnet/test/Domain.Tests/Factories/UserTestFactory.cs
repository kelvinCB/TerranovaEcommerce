using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Tests.Factories
{
    public static class UserTestFactory
    {
        public static User CreateUser()
        {
            return User.Create(
                id: Ulid.NewUlid(),
                firstName: "David",
                lastName: "Calcanio Hernandez",
                birthDate: new DateOnly(2001, 1, 1),
                gender: 'M',
                passwordHash: PasswordHash.From(new String('a', 64)),
                timestamp: new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero),
                emailAddress: Email.Create("test@example.com"),
                phoneNumber: PhoneNumber.Create("+18298881212")
            );
        }
    }
}