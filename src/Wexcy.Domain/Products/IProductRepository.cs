namespace Wexcy.Domain.Products;

public interface IProductRepository
{
    Task CreateAsync(Product entity);
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(Guid id);
    Task UpdateAsync(Product entity);
    Task<IQueryable<Product>> GetQueryableAsync();
    Task<(List<Product> Items, int TotalCount)> GetListAsync(
        int page, int pageSize, string? keyword, string? category, bool? includeDeleted);
    Task<int> SaveChangesAsync();
}