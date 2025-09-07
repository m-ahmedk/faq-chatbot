using FAQDemo.API.Models;

namespace FAQDemo.API.Models
{
    public class Order : BaseEntity
    {
        public int Id { get; set; }

        // FK to the user who placed the order
        public int UserId { get; set; }
        public AppUser User { get; set; }

        // Using Enum instead of string to avoid typos & ensure consistency
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Navigation property → list of products in this order
        public List<OrderItem> Items { get; set; } = new();
    }
}
