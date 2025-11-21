using Wexcy.Application.Products;
using Wexcy.Application.Products.DTOs;
using Wexcy.Domain.Products;
using Microsoft.EntityFrameworkCore;
using Wexcy.Infrastructure;
using Wexcy.Infrastructure.Products;

namespace Wexcy.UnitTests.Application;

public class ProductAppServiceTests
{
    private readonly WexcyDbContext _context;
    private readonly IProductRepository _repository;
    private readonly ProductManager _productManager;
    private readonly ProductAppService _appService;

    public ProductAppServiceTests()
    {
        var options = new DbContextOptionsBuilder<WexcyDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new WexcyDbContext(options);
        _repository = new ProductRepository(_context);
        _productManager = new ProductManager(_repository);
        _appService = new ProductAppService(_productManager, _repository);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProductAndReturnDto()
    {
        var input = new CreateProductRequest { Name = "Test Product", Category = "Test Category", Price = 100 };

        var result = await _appService.CreateAsync(input);

        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal("Test Category", result.Category);
        Assert.Equal(100, result.Price);
        Assert.NotNull(result.ConcurrencyStamp);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProductAndReturnDto()
    {
        var product = CreateProduct("Old Product", "Old Category", 100);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        var concurrencyStamp = product.ConcurrencyStamp;

        var input = new UpdateProductRequest 
        { 
            Name = "Updated Product", 
            Category = "Updated Category", 
            Price = 200,
            ConcurrencyStamp = concurrencyStamp
        };

        var result = await _appService.UpdateAsync(product.Id, input);

        Assert.NotNull(result);
        Assert.Equal("Updated Product", result.Name);
        Assert.Equal("Updated Category", result.Category);
        Assert.Equal(200, result.Price);
        Assert.NotEqual(concurrencyStamp, result.ConcurrencyStamp);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct()
    {
        var product = CreateProduct("Product", "Category", 100);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        await _appService.DeleteAsync(product.Id);

        var deletedProduct = await _repository.GetByIdAsync(product.Id);
        Assert.Null(deletedProduct); // Should be null due to soft delete filter
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProductDto_WhenProductExists()
    {
        var product = CreateProduct("Test Product", "Category", 100);
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();

        var result = await _appService.GetByIdAsync(product.Id);

        Assert.NotNull(result);
        Assert.Equal("Test Product", result.Name);
        Assert.Equal("Category", result.Category);
        Assert.Equal(100, result.Price);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenProductNotFound()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _appService.GetByIdAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnProductDtos()
    {
        var product1 = CreateProduct("Product 1", "Category", 100);
        var product2 = CreateProduct("Product 2", "Category", 200);
        await _context.Products.AddRangeAsync(product1, product2);
        await _context.SaveChangesAsync();

        var query = new GetProductsQuery { Page = 1, PageSize = 10 };
        var (items, totalCount) = await _appService.GetListAsync(query);

        Assert.Equal(2, items.Count);
        Assert.Equal(2, totalCount);
        Assert.Equal("Product 1", items[0].Name);
        Assert.Equal("Product 2", items[1].Name);
    }

    [Fact]
    public async Task GetListAsync_ShouldReturnEmptyList_WhenNoProductsFound()
    {
        var query = new GetProductsQuery { Page = 1, PageSize = 10 };
        var (items, totalCount) = await _appService.GetListAsync(query);

        Assert.Empty(items);
        Assert.Equal(0, totalCount);
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
