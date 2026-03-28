using Microsoft.AspNetCore.Mvc;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Services;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            _logger.LogInformation("User registration attempt for username: {Username}", registerDto.Username);
            
            var result = await _authService.RegisterAsync(registerDto);
            
            _logger.LogInformation("User registered successfully: {Username}", registerDto.Username);
            
            return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "注册成功"));
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            _logger.LogInformation("Login attempt for: {UsernameOrEmail}", loginDto.UsernameOrEmail);
            _logger.LogInformation("Password length: {Length}", loginDto.Password?.Length ?? 0);
            
            var result = await _authService.LoginAsync(loginDto);
            
            _logger.LogInformation("User logged in successfully: {Username}", result.User.Username);
            
            return Ok(ApiResponse<AuthResponseDto>.SuccessResult(result, "登录成功"));
        }
    }
}
