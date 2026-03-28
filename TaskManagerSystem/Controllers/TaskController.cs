using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerSystem.Common;
using TaskManagerSystem.Dtos;
using TaskManagerSystem.Services;

namespace TaskManagerSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
[Produces("application/json")]
public class TaskController : ControllerBase
{
    private readonly ITaskService _taskService;
    private readonly ILogger<TaskController> _logger;

    public TaskController(ITaskService taskService, ILogger<TaskController> logger)
    {
        _taskService = taskService;
        _logger = logger;
    }

    private long GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return long.Parse(userIdClaim!.Value);
    }

    /// <summary>
    /// 创建新任务
    /// </summary>
    /// <param name="createTaskDto">任务信息</param>
    /// <returns>创建的任务</returns>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> CreateTask([FromBody] CreateTaskDto createTaskDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 创建新任务", userId);
        
        var result = await _taskService.CreateTaskAsync(userId, createTaskDto);
        return Ok(ApiResponse<TaskResponseDto>.Ok(result, "任务创建成功"));
    }

    /// <summary>
    /// 获取任务列表（支持分页和状态筛选）
    /// </summary>
    /// <param name="request">分页和筛选参数</param>
    /// <returns>任务列表</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<TaskResponseDto>>>> GetTasks(
        [FromQuery] TaskPagedRequestDto request)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取任务列表", userId);
        
        var result = await _taskService.GetTasksAsync(userId, request);
        return Ok(ApiResponse<PagedResponseDto<TaskResponseDto>>.Ok(result, "获取任务列表成功"));
    }

    /// <summary>
    /// 根据ID获取任务
    /// </summary>
    /// <param name="id">任务ID</param>
    /// <returns>任务详情</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> GetTaskById(long id)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取任务 {TaskId}", userId, id);
        
        var result = await _taskService.GetTaskByIdAsync(userId, id);
        
        if (result == null)
        {
            return NotFound(ApiResponse<TaskResponseDto>.Error("任务不存在", 404));
        }
        
        return Ok(ApiResponse<TaskResponseDto>.Ok(result, "获取任务成功"));
    }

    /// <summary>
    /// 更新任务
    /// </summary>
    /// <param name="id">任务ID</param>
    /// <param name="updateTaskDto">更新的任务信息</param>
    /// <returns>更新后的任务</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TaskResponseDto>>> UpdateTask(
        long id, 
        [FromBody] UpdateTaskDto updateTaskDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 更新任务 {TaskId}", userId, id);
        
        var result = await _taskService.UpdateTaskAsync(userId, id, updateTaskDto);
        
        if (result == null)
        {
            return NotFound(ApiResponse<TaskResponseDto>.Error("任务不存在", 404));
        }
        
        return Ok(ApiResponse<TaskResponseDto>.Ok(result, "任务更新成功"));
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    /// <param name="id">任务ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTask(long id)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 删除任务 {TaskId}", userId, id);
        
        var result = await _taskService.DeleteTaskAsync(userId, id);
        
        if (!result)
        {
            return NotFound(ApiResponse<bool>.Error("任务不存在", 404));
        }
        
        return Ok(ApiResponse<bool>.Ok(true, "任务删除成功"));
    }
}
