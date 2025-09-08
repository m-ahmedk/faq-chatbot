using FAQDemo.API.Models;
using Pgvector;

namespace FAQDemo.API.Repositories.Interfaces
{
    public interface IFaqRepository
    {
        Task<Faq> AddAsync(Faq faq);                  // add new FAQ
        Task<List<Faq>> GetAllAsync();                // list all FAQs
        Task<Faq?> GetByIdAsync(int id);              // get single FAQ
        Task UpdateAsync(Faq faq);                    // update FAQ
        Task<bool> DeleteAsync(int id);               // soft delete

        // Semantic search → retrieve closest matches
        Task<List<Faq>> SearchAsync(Vector queryVector, int topN = 5);
    }
}
