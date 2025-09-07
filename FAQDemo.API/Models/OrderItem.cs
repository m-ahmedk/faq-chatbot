using FAQDemo.API.Models;

namespace FAQDemo.API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        // FK to parent order
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // FK to product being purchased
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        // Snapshot values (stored at purchase time)
        public int Quantity { get; set; }
        public double UnitPrice { get; set; } // price locked at order time
    }
}
