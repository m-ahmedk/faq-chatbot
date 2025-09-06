using FAQDemo.API.DTOs.Auth;
using FAQDemo.API.Models;

namespace FAQDemo.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AppUser> RegisterAsync(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
    }

}