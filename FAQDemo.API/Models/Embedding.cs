using Pgvector;

namespace FAQDemo.API.Models
{
    public class Embedding
    {
        public int Id { get; set; }

        // FK to Product
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // JSON string storing vector (array of floats)
        public Vector Vector { get; set; }

    }
}
