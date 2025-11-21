using Wexcy.Domain.Products;

namespace Wexcy.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(WexcyDbContext context)
    {
        if (context.Products.Any())
            return;

        var products = new List<Product>();
        var productType = typeof(Product);
        var constructor = productType.GetConstructor(
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            null,
            new[] { typeof(Guid), typeof(string), typeof(string), typeof(decimal) },
            null);

        var productData = new[]
        {
            ("Laptop", "Electronics", 999.99m),
            ("Mouse", "Electronics", 29.99m),
            ("Keyboard", "Electronics", 79.99m),
            ("Monitor", "Electronics", 299.99m),
            ("Desk Chair", "Furniture", 199.99m),
            ("Standing Desk", "Furniture", 449.99m),
            ("Notebook", "Stationery", 4.99m),
            ("Pen Set", "Stationery", 12.99m),
            ("Coffee Mug", "Kitchen", 14.99m),
            ("Water Bottle", "Kitchen", 19.99m)
        };

        foreach (var (name, category, price) in productData)
        {
            var product = constructor!.Invoke(new object[] { Guid.NewGuid(), name, category, price }) as Product;
            products.Add(product!);
        }

        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();
    }
}
