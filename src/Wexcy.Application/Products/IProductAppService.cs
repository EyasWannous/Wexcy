using Wexcy.Application.Products.DTOs;
using Wexcy.Domain.Products;

namespace Wexcy.Application.Products;

public interface IProductAppService
{
    Task<ProductDto> CreateAsync(CreateProductRequest input);
    Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest input);
    Task DeleteAsync(Guid id);
    Task<ProductDto> GetByIdAsync(Guid id);
    Task<(List<ProductDto> Items, int TotalCount)> GetListAsync(GetProductsQuery input);
}
