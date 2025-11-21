using Microsoft.EntityFrameworkCore;
using Wexcy.Domain.Products;

namespace Wexcy.Infrastructure;

public class WexcyDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WexcyDbContext).Assembly);
    }
}
