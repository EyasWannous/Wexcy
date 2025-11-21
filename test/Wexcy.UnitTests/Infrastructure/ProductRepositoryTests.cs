using Microsoft.EntityFrameworkCore;
using Wexcy.Domain.Products;
using Wexcy.Infrastructure;
using Wexcy.Infrastructure.Products;

namespace Wexcy.UnitTests.Infrastructure;

public class ProductRepositoryTests
{
    private readonly WexcyDbContext _context;
    private readonly ProductRepository _repository;

    public ProductRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<WexcyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new WexcyDbContext(options);
        _repository = new ProductRepository(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenProductExists()
    {
        var product = CreateProduct("Test Product", "Category", 10m);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal(product.Id, result.Id);
        Assert.Equal("Test Product", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldNotReturnDeletedProduct()
    {
        var product = CreateProduct("Deleted Product", "Category", 10m);
        var deleteMethod = typeof(Product).GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        deleteMethod!.Invoke(product, null);

        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var result = await _repository.GetByIdAsync(product.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllNonDeletedProducts()
    {
        var product1 = CreateProduct("Product 1", "Category", 10m);
        var product2 = CreateProduct("Product 2", "Category", 20m);
        var product3 = CreateProduct("Product 3", "Category", 30m);

        await _context.Products.AddRangeAsync(product1, product2, product3);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public async Task GetAllAsync_ShouldNotReturnDeletedProducts()
    {
        var product1 = CreateProduct("Product 1", "Category", 10m);
        var product2 = CreateProduct("Product 2", "Category", 20m);
        var deleteMethod = typeof(Product).GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        deleteMethod!.Invoke(product2, null);

        await _context.Products.AddRangeAsync(product1, product2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Single(result);
        Assert.Equal("Product 1", result[0].Name);
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnFilteredProducts_ByKeyword()
    {
        var product1 = CreateProduct("Apple", "Fruit", 1.0m);
        var product2 = CreateProduct("Banana", "Fruit", 1.5m);
        var product3 = CreateProduct("Carrot", "Vegetable", 2.0m);

        await _context.Products.AddRangeAsync(product1, product2, product3);
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _repository.GetListAsync(1, 10, "app", null, null);

        Assert.Single(items);
        Assert.Equal("Apple", items[0].Name);
        Assert.Equal(1, totalCount);
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnFilteredProducts_ByCategory()
    {
        var product1 = CreateProduct("Apple", "Fruit", 1.0m);
        var product2 = CreateProduct("Banana", "Fruit", 1.5m);
        var product3 = CreateProduct("Carrot", "Vegetable", 2.0m);

        await _context.Products.AddRangeAsync(product1, product2, product3);
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _repository.GetListAsync(1, 10, null, "Fruit", null);

        Assert.Equal(2, items.Count);
        Assert.Equal(2, totalCount);
    }

    [Fact]
    public async Task GetListAsync_ShouldSupportPagination()
    {
        for (int i = 1; i <= 15; i++)
        {
            var product = CreateProduct($"Product {i}", "Category", i);
            await _context.Products.AddAsync(product);
        }
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _repository.GetListAsync(2, 5, null, null, null);

        Assert.Equal(5, items.Count);
        Assert.Equal(15, totalCount);
    }

    [Fact]
    public async Task GetListAsync_ShouldIncludeDeletedProducts_WhenRequested()
    {
        var product1 = CreateProduct("Product 1", "Category", 10m);
        var product2 = CreateProduct("Product 2", "Category", 20m);
        var deleteMethod = typeof(Product).GetMethod("Delete", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        deleteMethod!.Invoke(product2, null);

        await _context.Products.AddRangeAsync(product1, product2);
        await _context.SaveChangesAsync();

        var (items, totalCount) = await _repository.GetListAsync(1, 10, null, null, true);

        Assert.Equal(2, items.Count);
        Assert.Equal(2, totalCount);
    }

    private static Product CreateProduct(string name, string category, decimal price)
    {
        return Activator.CreateInstance(
            typeof(Product),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            null,
            [Guid.NewGuid(), name, category, price],
            null) as Product ?? throw new InvalidOperationException("Failed to create product");
    }
}
