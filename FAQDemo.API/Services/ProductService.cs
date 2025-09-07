using FAQDemo.API.Models;
using FAQDemo.API.Repositories.Interfaces;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IEmbeddingService _embeddingService; // inject embedding
    public ProductService(IProductRepository repository, IEmbeddingService embeddingService)
    {
        _repository = repository;
        _embeddingService = embeddingService;
    }

    /// <summary>
    /// Create a product and generate its embedding.
    /// </summary>
    public async Task<Product> AddAsync(Product product)
    {
        var exists = await _repository.ExistsByNameAsync(product.Name);
        if (exists)
            throw new InvalidOperationException("Product name must be unique.");

        await _repository.AddAsync(product);

        // generate embedding right after product creation
        await _embeddingService.CreateProductEmbeddingAsync(product);

        return product;
    }

    /// <summary>
    /// Delete a product and remove its embedding.
    /// </summary>
    public async Task DeleteAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid product ID.");

        var deleted = await _repository.DeleteAsync(id);
        if (!deleted)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        // remove embedding if exists
        await _embeddingService.DeleteProductEmbeddingAsync(id);
    }

    /// <summary>
    /// Get all products (normal query).
    /// </summary>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }

    /// <summary>
    /// Get a product by ID.
    /// </summary>
    public async Task<Product> GetByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Invalid product ID.");

        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        return product;
    }

    /// <summary>
    /// Update a product and regenerate its embedding.
    /// </summary>
    public async Task UpdateAsync(Product product)
    {
        if (product == null || product.Id <= 0)
            throw new ArgumentException("Invalid product.");

        var updated = await _repository.UpdateAsync(product);
        if (!updated)
            throw new InvalidOperationException("Update failed. Try again later.");

        // regenerate embedding when product changes
        await _embeddingService.CreateProductEmbeddingAsync(product);
    }

}
