using Domain.Entities;

namespace Domain.Tests.Factories;

public static class RoleTestFactory
{
    public static Role CreateRole()
    {
        return Role.Create(
            id: Ulid.NewUlid(),
            name: "Admin",
            description: "Administrator role",
            timestamp: new DateTimeOffset(2022, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );
    }
}