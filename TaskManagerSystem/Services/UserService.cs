using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagerSystem.Common;
using TaskManagerSystem.Dtos;
using TaskManagerSystem.Models;
using TaskManagerSystem.Repositories;

namespace TaskManagerSystem.Services;

public class UserService : IUserService
{
    private readonly IRepository<User> _userRepository;
    private readonly JwtSettings _jwtSettings;

    public UserService(IRepository<User> userRepository, IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await UsernameExistsAsync(registerDto.Username))
        {
            throw new Exception("用户名已存在");
        }

        if (await EmailExistsAsync(registerDto.Email))
        {
            throw new Exception("邮箱已被注册");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            Email = registerDto.Email,
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.AddAsync(user);

        return await GenerateLoginResponseAsync(createdUser);
    }

    public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
    {
        var users = await _userRepository.FindAsync(u => 
            u.Username == loginDto.UsernameOrEmail || u.Email == loginDto.UsernameOrEmail);
        
        var user = users.FirstOrDefault();

        if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new Exception("用户名或密码错误");
        }

        return await GenerateLoginResponseAsync(user);
    }

    public async Task<UserResponseDto?> GetUserByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToUserResponseDto(user);
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userRepository.ExistsAsync(u => u.Username == username);
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userRepository.ExistsAsync(u => u.Email == email);
    }

    private async Task<LoginResponseDto> GenerateLoginResponseAsync(User user)
    {
        var token = GenerateJwtToken(user);
        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

        return new LoginResponseDto
        {
            Token = token,
            User = MapToUserResponseDto(user),
            ExpiresAt = expiresAt
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private UserResponseDto MapToUserResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt
        };
    }
}
