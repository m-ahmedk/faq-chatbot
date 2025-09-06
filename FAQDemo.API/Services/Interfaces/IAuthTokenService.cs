using FAQDemo.API.Models;

namespace FAQDemo.API.Services.Interfaces
{
    public interface IAuthTokenService
    {
        string CreateToken(AppUser user);
    }
}
