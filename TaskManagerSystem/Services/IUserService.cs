using TaskManagerSystem.Dtos;

namespace TaskManagerSystem.Services;

public interface IUserService
{
    Task<LoginResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
    Task<UserResponseDto?> GetUserByIdAsync(long id);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> EmailExistsAsync(string email);
}
