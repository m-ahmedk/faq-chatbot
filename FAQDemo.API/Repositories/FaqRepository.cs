using FAQDemo.API.Data;
using FAQDemo.API.Models;
using FAQDemo.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace FAQDemo.API.Repositories
{
    public class FaqRepository : IFaqRepository
    {
        private readonly AppDbContext _context;

        public FaqRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Faq> AddAsync(Faq faq)
        {
            _context.Faqs.Add(faq);
            await _context.SaveChangesAsync();
            return faq;
        }

        public async Task<List<Faq>> GetAllAsync()
        {
            return await _context.Faqs.AsNoTracking().ToListAsync();
        }

        public async Task<Faq?> GetByIdAsync(int id)
        {
            return await _context.Faqs.FindAsync(id);
        }

        public async Task UpdateAsync(Faq faq)
        {
            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var faq = await _context.Faqs.FirstOrDefaultAsync(f => f.Id == id);
            if (faq == null) return false;

            faq.IsDeleted = true;
            faq.DeletedAt = DateTime.UtcNow;
            faq.LastModifiedAt = DateTime.UtcNow;

            _context.Faqs.Update(faq);
            await _context.SaveChangesAsync();
            return true;
        }

        // Semantic vector search
        public async Task<List<Faq>> SearchAsync(Vector queryVector, int topN = 5)
        {
            return await _context.Faqs
                .OrderBy(f => f.Vector.CosineDistance(queryVector))
                .Take(topN)
                .ToListAsync();
        }
    }
}
