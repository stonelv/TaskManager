using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Services;
using TaskStatus = TaskManager.API.Models.TaskStatus;

namespace TaskManager.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(userIdClaim!);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResultDto<TaskDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTasks(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] TaskStatus? status = null)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Getting tasks for user: {UserId}, page: {PageNumber}, status: {Status}", 
                userId, pageNumber, status);

            var result = await _taskService.GetUserTasksAsync(userId, pageNumber, pageSize, status);
            
            return Ok(ApiResponse<PagedResultDto<TaskDto>>.SuccessResult(result, "获取任务列表成功"));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Getting task {TaskId} for user: {UserId}", id, userId);

            var task = await _taskService.GetTaskByIdAsync(id, userId);
            
            return Ok(ApiResponse<TaskDto>.SuccessResult(task, "获取任务成功"));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto createTaskDto)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Creating task for user: {UserId}", userId);

            var task = await _taskService.CreateTaskAsync(createTaskDto, userId);
            
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, 
                ApiResponse<TaskDto>.SuccessResult(task, "任务创建成功"));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] UpdateTaskDto updateTaskDto)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Updating task {TaskId} for user: {UserId}", id, userId);

            var task = await _taskService.UpdateTaskAsync(id, updateTaskDto, userId);
            
            return Ok(ApiResponse<TaskDto>.SuccessResult(task, "任务更新成功"));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetCurrentUserId();
            _logger.LogInformation("Deleting task {TaskId} for user: {UserId}", id, userId);

            await _taskService.DeleteTaskAsync(id, userId);
            
            return Ok(ApiResponse.SuccessResult("任务删除成功"));
        }
    }
}
