using Microsoft.AspNetCore.Mvc;
using TaskManagerSystem.Common;
using TaskManagerSystem.Dtos;
using TaskManagerSystem.Services;

namespace TaskManagerSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /// <summary>
    /// 用户注册
    /// </summary>
    /// <param name="registerDto">注册信息</param>
    /// <returns>注册成功返回用户信息和Token</returns>
    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Register([FromBody] RegisterDto registerDto)
    {
        _logger.LogInformation("用户注册请求: {Username}", registerDto.Username);
        
        var result = await _userService.RegisterAsync(registerDto);
        return Ok(ApiResponse<LoginResponseDto>.Ok(result, "注册成功"));
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    /// <param name="loginDto">登录信息</param>
    /// <returns>登录成功返回用户信息和Token</returns>
    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("用户登录请求: {UsernameOrEmail}", loginDto.UsernameOrEmail);
        
        var result = await _userService.LoginAsync(loginDto);
        return Ok(ApiResponse<LoginResponseDto>.Ok(result, "登录成功"));
    }
}
