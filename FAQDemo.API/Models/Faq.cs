using FAQDemo.API.Models;
using Pgvector;

namespace FAQDemo.API.Models
{
    public class Faq : BaseEntity
    {
        public int Id { get; set; }

        // User-facing question (what customers ask)
        public string Question { get; set; } = string.Empty;

        // The actual answer (what support would reply with)
        public string Answer { get; set; } = string.Empty;

        // Embedding vector (semantic representation of Q + A)
        public Vector Vector { get; set; } = null!;
    }
}
