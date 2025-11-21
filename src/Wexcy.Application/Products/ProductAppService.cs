using Wexcy.Application.Products.DTOs;
using Wexcy.Domain.Products;

namespace Wexcy.Application.Products;

public class ProductAppService : IProductAppService
{
    private readonly ProductManager _productManager;
    private readonly IProductRepository _productRepository;

    public ProductAppService(ProductManager productManager, IProductRepository productRepository)
    {
        _productManager = productManager;
        _productRepository = productRepository;
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest input)
    {
        var product = await _productManager.CreateAsync(input.Name, input.Category, input.Price);
        await _productRepository.CreateAsync(product);
        await _productRepository.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest input)
    {
        var product = await _productManager.UpdateAsync(id, input.Name, input.Category, input.Price, input.ConcurrencyStamp);
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
        return MapToDto(product);
    }

    public async Task DeleteAsync(Guid id)
    {
        var product = await _productManager.DeleteAsync(id);
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    public async Task<ProductDto> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
        {
            throw new KeyNotFoundException($"Product with id {id} not found.");
        }
        return MapToDto(product);
    }

    public async Task<(List<ProductDto> Items, int TotalCount)> GetListAsync(GetProductsQuery input)
    {
        var (items, totalCount) = await _productRepository.GetListAsync(
            input.Page, input.PageSize, input.Keyword, input.Category, input.IncludeDeleted);
        
        return (items.Select(MapToDto).ToList(), totalCount);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Category = product.Category,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            ConcurrencyStamp = product.ConcurrencyStamp
        };
    }
}
