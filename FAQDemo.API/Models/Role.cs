namespace FAQDemo.API.Models
{
    public class Role : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<UserRole> UserRoles { get; set; } = new();
    }
}
