using Microsoft.EntityFrameworkCore;
using Pgvector; // Provides Vector type (float[] wrapper for PostgreSQL pgvector column)
using Pgvector.EntityFrameworkCore; // Adds EF Core extensions for pgvector (like CosineDistance)
using FAQDemo.API.Data;
using FAQDemo.API.Models;
using FAQDemo.API.Repositories.Interfaces;

namespace FAQDemo.API.Repositories
{
    /// <summary>
    /// Repository that handles database operations for product embeddings.
    /// </summary>
    public class EmbeddingRepository : IEmbeddingRepository
    {
        private readonly AppDbContext _db;

        public EmbeddingRepository(AppDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// Add a new embedding into the database.
        /// </summary>
        public async Task AddAsync(Embedding embedding)
        {
            _db.Embeddings.Add(embedding);
            await _db.SaveChangesAsync();
        }

        /// <summary>
        /// Search embeddings by similarity using cosine distance.
        /// 
        /// Cosine Distance:
        /// - Measures how similar two vectors are by looking at the angle between them.
        /// - Range: 0 = identical, 1 = very different.
        /// - In embeddings, closer (smaller distance) = more semantically similar.
        /// 
        /// Example:
        ///   "Coca Cola" and "Pepsi" -> vectors close together.
        ///   "Coca Cola" and "Laptop" -> vectors far apart.
        /// </summary>
        public async Task<List<Embedding>> GetSimilarEmbeddingsAsync(Vector queryVector, int topN)
        {
            return await _db.Embeddings
                .OrderBy(e => e.Vector.CosineDistance(queryVector)) // ORDER BY vector similarity
                .Take(topN)                                         // Limit results
                .Include(e => e.Product)                            // Also load linked Product
                .ToListAsync();
        }

        /// <summary>
        /// Get an embedding for a specific product (by productId).
        /// Used when updating or deleting.
        /// </summary>
        public async Task<Embedding?> GetByProductIdAsync(int productId)
        {
            return await _db.Embeddings.FirstOrDefaultAsync(e => e.ProductId == productId);
        }

        /// <summary>
        /// Delete an embedding from the database.
        /// </summary>
        public async Task DeleteAsync(Embedding embedding)
        {
            _db.Embeddings.Remove(embedding);
            await _db.SaveChangesAsync();
        }
    }
}
