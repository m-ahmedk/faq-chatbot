using System.Security.Claims;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? UserId
        {
            get
            {
                var claim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return int.TryParse(claim, out var id) ? id : null;
            }
        }

        public string? Email =>
            _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

        public List<string> Roles =>
            _httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? new();
    }
}
