using Domain.Entities;

namespace Domain.Tests.Factories;

public static class CartTestFactory
{
    public static Cart CreateCart()
    {
        return Cart.Create(
            id: Ulid.NewUlid(),
            userId: Ulid.NewUlid(),
            timestamp: new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero)
        );
    }
}