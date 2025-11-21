using Microsoft.EntityFrameworkCore;
using Wexcy.Domain.Products;

namespace Wexcy.Infrastructure.Products;

public class ProductRepository(WexcyDbContext context) : IProductRepository
{
    private readonly DbSet<Product> _dbSet = context.Set<Product>();

    public Task<Product?> GetByIdAsync(Guid id)
        => _dbSet.FirstOrDefaultAsync(x => x.Id == id);

    public Task<List<Product>> GetAllAsync()
        => _dbSet.ToListAsync();

    public Task CreateAsync(Product entity) => _dbSet.AddAsync(entity).AsTask();

    public Task UpdateAsync(Product entity)
        => Task.FromResult(_dbSet.Update(entity));

    public Task DeleteAsync(Product entity)
        => Task.FromResult(_dbSet.Remove(entity));

    public Task<IQueryable<Product>> GetQueryableAsync()
        => Task.FromResult(_dbSet.AsQueryable());

    public async Task<(List<Product> Items, int TotalCount)> GetListAsync(
        int page, int pageSize, string? keyword, string? category, bool? includeDeleted)
    {
        var query = _dbSet.AsQueryable();

        if (includeDeleted.HasValue && includeDeleted.Value)
        {
            query = query.IgnoreQueryFilters();
        }

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.Name.Contains(keyword, StringComparison.CurrentCultureIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => x.Category.Equals(category, StringComparison.CurrentCultureIgnoreCase));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public Task<int> SaveChangesAsync()
        => context.SaveChangesAsync();
}
