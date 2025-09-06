using Pgvector;
using FAQDemo.API.Models;

namespace FAQDemo.API.Repositories.Interfaces
{
    public interface IEmbeddingRepository
    {
        Task AddAsync(Embedding embedding);
        Task<List<Embedding>> GetSimilarEmbeddingsAsync(Vector queryVector, int topN);
        Task<Embedding?> GetByProductIdAsync(int productId);
        Task DeleteAsync(Embedding embedding);
    }
}