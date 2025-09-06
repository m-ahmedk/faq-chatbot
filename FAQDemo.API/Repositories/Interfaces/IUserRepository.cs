using FAQDemo.API.Models;

namespace FAQDemo.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email);
        Task<AppUser> AddAsync(AppUser user);
    }
}
