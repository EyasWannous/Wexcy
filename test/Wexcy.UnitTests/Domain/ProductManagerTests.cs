using Wexcy.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Wexcy.Infrastructure;
using Wexcy.Infrastructure.Products;

namespace Wexcy.UnitTests.Domain;

public class ProductManagerTests
{
    private readonly WexcyDbContext _context;
    private readonly IProductRepository _repository;
    private readonly ProductManager _manager;

    public ProductManagerTests()
    {
        var options = new DbContextOptionsBuilder<WexcyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new WexcyDbContext(options);
        _repository = new ProductRepository(_context);
        _manager = new ProductManager(_repository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct_WhenNameIsUnique()
    {
        var result = await _manager.CreateAsync("New Product", "Category", 10m);

        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal("Category", result.Category);
        Assert.Equal(10m, result.Price);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowException_WhenNameIsNotUnique()
    {
        const string name = "Existing Product";
        var existingProduct = CreateProduct(name, "Category", 10m);
        await _context.Products.AddAsync(existingProduct);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _manager.CreateAsync(name, "Category", 20m));
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct_WhenConcurrencyStampMatches()
    {
        var product = CreateProduct("Old Name", "Category", 10m);
        var concurrencyStamp = product.ConcurrencyStamp;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var result = await _manager.UpdateAsync(product.Id, "New Name", "New Category", 20m, concurrencyStamp);

        Assert.Equal("New Name", result.Name);
        Assert.Equal("New Category", result.Category);
        Assert.Equal(20m, result.Price);
        Assert.NotEqual(concurrencyStamp, result.ConcurrencyStamp);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenConcurrencyStampDoesNotMatch()
    {
        var product = CreateProduct("Product", "Category", 10m);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        const string wrongStamp = "wrong-stamp";

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _manager.UpdateAsync(product.Id, "New Name", "Category", 20m, wrongStamp));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenProductNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _manager.UpdateAsync(Guid.NewGuid(), "Name", "Category", 10m, "stamp"));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowException_WhenNewNameIsNotUnique()
    {
        var product1 = CreateProduct("Product 1", "Category", 10m);
        var product2 = CreateProduct("Product 2", "Category", 20m);
        var concurrencyStamp = product1.ConcurrencyStamp;
        await _context.Products.AddRangeAsync(product1, product2);
        await _context.SaveChangesAsync();

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _manager.UpdateAsync(product1.Id, "Product 2", "Category", 15m, concurrencyStamp));
    }

    [Fact]
    public async Task UpdateAsync_ShouldNotCheckUniqueness_WhenNameIsNotChanged()
    {
        var product = CreateProduct("Product", "Category", 10m);
        var concurrencyStamp = product.ConcurrencyStamp;
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var result = await _manager.UpdateAsync(product.Id, "Product", "New Category", 20m, concurrencyStamp);

        Assert.Equal("Product", result.Name);
        Assert.Equal("New Category", result.Category);
    }

    [Fact]
    public async Task DeleteAsync_ShouldMarkProductAsDeleted()
    {
        var product = CreateProduct("Product", "Category", 10m);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var result = await _manager.DeleteAsync(product.Id);

        Assert.True(result.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowException_WhenProductNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _manager.DeleteAsync(Guid.NewGuid()));
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
