using Domain.Entities;

namespace Common.Tests.Factories;

public static class UserRoleTestFactory
{
    public static UserRole CreateUserRole(Ulid? userId, Ulid? roleId)
    {
        return UserRole.Create(
            userId: userId ?? Ulid.NewUlid(),
            roleId: roleId ?? Ulid.NewUlid(),
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );
    }
}