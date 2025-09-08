using FAQDemo.API.Models;

namespace FAQDemo.API.Services.Interfaces
{
    public interface IFaqService
    {
        Task<Faq> AddAsync(Faq faq);
        Task<List<Faq>> GetAllAsync();
        Task<Faq?> GetByIdAsync(int id);
        Task UpdateAsync(Faq faq);
        Task<bool> DeleteAsync(int id);

        // The heart of chatbot: semantic search
        Task<List<Faq>> SearchAsync(string query, int topN = 5);
        Task<string> AskAsync(string question, int topN = 3);

    }
}
