using FAQDemo.API.DTOs.Auth;
using FAQDemo.API.Helpers;
using FAQDemo.API.Models;
using FAQDemo.API.Repositories.Interfaces;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAuthTokenService _tokenService;

        public AuthService(IUserRepository userRepository, IAuthTokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<AppUser> RegisterAsync(RegisterDto dto)
        {
            if (await _userRepository.ExistsByEmailAsync(dto.Email))
                throw new InvalidOperationException("Email already exists");

            var (hash, salt) = HashHelper.HashPassword(dto.Password);

            var user = new AppUser
            {
                Email = dto.Email.ToLowerInvariant(),
                PasswordHash = hash,
                PasswordStamp = salt
            };

            await _userRepository.AddAsync(user);

            return user;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var isValid = HashHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordStamp);
            if (!isValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return _tokenService.CreateToken(user);
        }
    }
}
