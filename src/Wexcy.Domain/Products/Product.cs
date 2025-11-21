namespace Wexcy.Domain.Products;

public class Product
{
    private Product() { }

    internal Product(Guid id, string name, string category, decimal price)
    {
        Id = id;
        Name = name;

        SetCategory(category);
        SetPrice(price);
    }

    internal void SetCategory(string category)
    {
        Category = category;
    }

    internal void SetName(string name)
    {
        Name = name;
    }

    internal void SetPrice(decimal price)
    {
        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price must be greater than 0.");
        }

        Price = price;
    }

    internal void Delete()
    {
        IsDeleted = true;
    }

    internal void SetConcurrencyStamp(string currentStamp)
    {
        if (ConcurrencyStamp.Equals(currentStamp) is false)
        {
            throw new InvalidOperationException("The product has been modified by another user. Please refresh and try again.");
        }

        ConcurrencyStamp = Guid.NewGuid().ToString();
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Category { get; private set; }
    public decimal Price { get; private set; }

    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public bool IsDeleted { get; private set; }
    public string ConcurrencyStamp { get; private set; } = Guid.NewGuid().ToString();
}
