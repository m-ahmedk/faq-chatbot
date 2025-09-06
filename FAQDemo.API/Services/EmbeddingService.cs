using Microsoft.EntityFrameworkCore;
using OpenAI;
using OpenAI.Embeddings;
using Pgvector;
using FAQDemo.API.Models;
using FAQDemo.API.Repositories.Interfaces;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Services
{
    public class EmbeddingService : IEmbeddingService
    {
        private readonly IEmbeddingRepository _repository;
        private readonly EmbeddingClient _embeddingClient;

        public EmbeddingService(IEmbeddingRepository repository, IConfiguration config)
        {
            _repository = repository;

            var apiKey = config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OpenAI API key not found.");

            var client = new OpenAIClient(apiKey);
            _embeddingClient = client.GetEmbeddingClient("text-embedding-3-small");
        }

        public async Task CreateProductEmbeddingAsync(Product product)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(product.Name);
            var vector = response.Value.ToFloats().ToArray();

            var embedding = new Embedding
            {
                ProductId = product.Id,
                Vector = new Vector(vector)
            };

            await _repository.AddAsync(embedding);
        }

        public async Task<List<Product>> SearchProductsAsync(string query, int topN = 5)
        {
            var response = await _embeddingClient.GenerateEmbeddingAsync(query);
            var queryVector = new Vector(response.Value.ToFloats().ToArray());

            var results = await _repository.GetSimilarEmbeddingsAsync(queryVector, topN);

            return results.Select(e => e.Product).ToList();
        }

        public async Task DeleteProductEmbeddingAsync(int productId)
        {
            var existing = await _repository.GetByProductIdAsync(productId);
            if (existing != null)
                await _repository.DeleteAsync(existing);
        }
    }
}
