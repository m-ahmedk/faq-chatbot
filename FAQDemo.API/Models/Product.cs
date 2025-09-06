namespace FAQDemo.API.Models
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public double Quantity { get; set; }
    }
}
