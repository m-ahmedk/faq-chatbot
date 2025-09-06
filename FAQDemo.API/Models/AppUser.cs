namespace FAQDemo.API.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordStamp { get; set; }
        public List<UserRole> UserRoles { get; set; } = new();
    }
}