using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FAQDemo.API.Data;
using FAQDemo.API.Models;
using FAQDemo.API.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FAQDemo.API.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AuthTokenService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // add user role to claims
            var userWithRoles = _context.Users
                                .Include(u => u.UserRoles) // from userroles
                                .ThenInclude(u => u.Role) // get role
                                .FirstOrDefault(u => u.Id == user.Id); // of the following user

            if(userWithRoles != null)
            {
                foreach (var role in userWithRoles.UserRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role.Role.Name));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
