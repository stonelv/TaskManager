using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagerSystem.Common;
using TaskManagerSystem.Dtos;
using TaskManagerSystem.Services;

namespace TaskManagerSystem.Controllers;

[ApiController]
[Route("api/")]
[Authorize]
[Produces("application/json")]
public class TaskCommentController : ControllerBase
{
    private readonly ITaskCommentService _commentService;
    private readonly ILogger<TaskCommentController> _logger;

    public TaskCommentController(ITaskCommentService commentService, ILogger<TaskCommentController> logger)
    {
        _commentService = commentService;
        _logger = logger;
    }

    private long GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        return long.Parse(userIdClaim!.Value);
    }

    /// <summary>
    /// 创建任务评论
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="createCommentDto">评论内容</param>
    /// <returns>创建的评论</returns>
    [HttpPost("tasks/{taskId}/comments")]
    public async Task<ActionResult<ApiResponse<CommentResponseDto>>> CreateComment(
        long taskId,
        [FromBody] CreateCommentDto createCommentDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 为任务 {TaskId} 创建评论", userId, taskId);

        try
        {
            var result = await _commentService.CreateCommentAsync(userId, taskId, createCommentDto);
            return Ok(ApiResponse<CommentResponseDto>.Ok(result, "评论创建成功"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<CommentResponseDto>.Error(ex.Message, 404));
        }
    }

    /// <summary>
    /// 获取任务的评论列表（支持分页）
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="request">分页参数</param>
    /// <returns>评论列表</returns>
    [HttpGet("tasks/{taskId}/comments")]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<CommentResponseDto>>>> GetComments(
        long taskId,
        [FromQuery] CommentPagedRequestDto request)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取任务 {TaskId} 的评论列表", userId, taskId);

        try
        {
            var result = await _commentService.GetCommentsByTaskIdAsync(userId, taskId, request);
            return Ok(ApiResponse<PagedResponseDto<CommentResponseDto>>.Ok(result, "获取评论列表成功"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<PagedResponseDto<CommentResponseDto>>.Error(ex.Message, 404));
        }
    }

    /// <summary>
    /// 删除任务评论
    /// </summary>
    /// <param name="taskId">任务ID</param>
    /// <param name="commentId">评论ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("tasks/{taskId}/comments/{commentId}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(
        long taskId,
        long commentId)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 删除任务 {TaskId} 的评论 {CommentId}", userId, taskId, commentId);

        try
        {
            var result = await _commentService.DeleteCommentAsync(userId, taskId, commentId);
            
            if (!result)
            {
                return NotFound(ApiResponse<bool>.Error("评论不存在", 404));
            }

            return Ok(ApiResponse<bool>.Ok(true, "评论删除成功"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<bool>.Error(ex.Message, 404));
        }
    }
}