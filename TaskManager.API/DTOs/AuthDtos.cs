using System.ComponentModel.DataAnnotations;

namespace TaskManager.API.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "用户名不能为空")]
        [MinLength(3, ErrorMessage = "用户名至少需要3个字符")]
        [MaxLength(50, ErrorMessage = "用户名不能超过50个字符")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        [MinLength(6, ErrorMessage = "密码至少需要6个字符")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginDto
    {
        [Required(ErrorMessage = "用户名或邮箱不能为空")]
        public string UsernameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "密码不能为空")]
        public string Password { get; set; } = string.Empty;
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
