using Microsoft.EntityFrameworkCore;

namespace Wexcy.Domain.Products;

public class ProductManager
{
    private readonly IProductRepository _productRepository;

    public ProductManager(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Product> CreateAsync(string name, string category, decimal price)
    {
        await CheckUniqueNameAsync(name);

        var newProduct = new Product(Guid.NewGuid(), name, category, price);
        
        return newProduct;
    }

    public async Task<Product> UpdateAsync(Guid id, string name, string category, decimal price, string concurrencyStamp)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            throw new KeyNotFoundException($"Product with id {id} not found.");

        product.SetConcurrencyStamp(concurrencyStamp);

        if (!product.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
        {
            await CheckUniqueNameAsync(name);
        }

        product.SetName(name);
        product.SetCategory(category);
        product.SetPrice(price);

        return product;
    }

    public async Task<Product> DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product is null)
            throw new KeyNotFoundException($"Product with id {id} not found.");

        product.Delete();
        return product;
    }



    private async Task CheckUniqueNameAsync(string name)
    {
        var query = await _productRepository.GetQueryableAsync();

        var isNameExists = await query.AnyAsync(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));

        if (isNameExists)
            throw new ArgumentException($"Product with name '{name}' already exists.");
    }
}
