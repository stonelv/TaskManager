using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;

namespace TaskManager.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(IRepository<User> userRepository, IOptions<JwtSettings> jwtSettings)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            // Check if username already exists
            if (await _userRepository.ExistsAsync(u => u.Username == registerDto.Username))
            {
                throw new InvalidOperationException("用户名已存在");
            }

            // Check if email already exists
            if (await _userRepository.ExistsAsync(u => u.Email == registerDto.Email))
            {
                throw new InvalidOperationException("邮箱已被注册");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = registerDto.Username,
                Email = registerDto.Email,
                PasswordHash = HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt
                }
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var user = await _userRepository.FindAsync(u => 
                u.Username == loginDto.UsernameOrEmail || 
                u.Email == loginDto.UsernameOrEmail);
            
            var foundUser = user.FirstOrDefault();

            if (foundUser == null)
            {
                throw new UnauthorizedAccessException("用户名或密码错误");
            }

            // Verify password
            if (!VerifyPassword(loginDto.Password, foundUser.PasswordHash))
            {
                throw new UnauthorizedAccessException("用户名或密码错误");
            }

            var token = GenerateJwtToken(foundUser);

            return new AuthResponseDto
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
                User = new UserDto
                {
                    Id = foundUser.Id,
                    Username = foundUser.Username,
                    Email = foundUser.Email,
                    CreatedAt = foundUser.CreatedAt
                }
            };
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string HashPassword(string password)
        {
            using var hmac = new HMACSHA256();
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            var salt = hmac.Key;
            
            var hashBytes = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, hashBytes, salt.Length, hash.Length);
            
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            try
            {
                var hashBytes = Convert.FromBase64String(passwordHash);
                var salt = new byte[32];
                var hash = new byte[hashBytes.Length - 32];
                
                Buffer.BlockCopy(hashBytes, 0, salt, 0, 32);
                Buffer.BlockCopy(hashBytes, 32, hash, 0, hash.Length);
                
                using var hmac = new HMACSHA256(salt);
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                
                return computedHash.SequenceEqual(hash);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"VerifyPassword error: {ex.Message}");
                return false;
            }
        }
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryInHours { get; set; } = 24;
    }
}
