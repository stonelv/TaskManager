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
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly ILogger<CommentController> _logger;

    public CommentController(ICommentService commentService, ILogger<CommentController> logger)
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
    /// <param name="createCommentDto">评论信息</param>
    /// <returns>创建的评论</returns>
    /// <response code="200">评论创建成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="401">未授权访问</response>
    /// <response code="404">任务不存在或无权限操作</response>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentResponseDto>>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 为任务 {TaskId} 创建评论", userId, createCommentDto.TaskId);
        
        try
        {
            var result = await _commentService.CreateCommentAsync(userId, createCommentDto);
            return Ok(ApiResponse<CommentResponseDto>.Ok(result, "评论创建成功"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "用户 {UserId} 尝试为不存在的任务 {TaskId} 创建评论", userId, createCommentDto.TaskId);
            return NotFound(ApiResponse<CommentResponseDto>.Error(ex.Message, 404));
        }
    }

    /// <summary>
    /// 获取任务评论列表（支持分页）
    /// </summary>
    /// <param name="request">分页和任务ID参数</param>
    /// <returns>评论列表</returns>
    /// <response code="200">获取评论列表成功</response>
    /// <response code="400">请求参数无效</response>
    /// <response code="401">未授权访问</response>
    /// <response code="404">任务不存在或无权限操作</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<CommentResponseDto>>>> GetComments(
        [FromQuery] CommentPagedRequestDto request)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取任务 {TaskId} 的评论列表", userId, request.TaskId);
        
        try
        {
            var result = await _commentService.GetCommentsAsync(userId, request);
            return Ok(ApiResponse<PagedResponseDto<CommentResponseDto>>.Ok(result, "获取评论列表成功"));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "用户 {UserId} 尝试获取不存在的任务 {TaskId} 的评论列表", userId, request.TaskId);
            return NotFound(ApiResponse<PagedResponseDto<CommentResponseDto>>.Error(ex.Message, 404));
        }
    }

    /// <summary>
    /// 根据ID获取评论
    /// </summary>
    /// <param name="id">评论ID</param>
    /// <returns>评论详情</returns>
    /// <response code="200">获取评论成功</response>
    /// <response code="401">未授权访问</response>
    /// <response code="403">无权限查看该评论</response>
    /// <response code="404">评论不存在</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CommentResponseDto>>> GetCommentById(long id)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取评论 {CommentId}", userId, id);
        
        try
        {
            var result = await _commentService.GetCommentByIdAsync(userId, id);
            
            if (result == null)
            {
                return NotFound(ApiResponse<CommentResponseDto>.Error("评论不存在", 404));
            }
            
            return Ok(ApiResponse<CommentResponseDto>.Ok(result, "获取评论成功"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "用户 {UserId} 无权限查看评论 {CommentId}", userId, id);
            return StatusCode(403, ApiResponse<CommentResponseDto>.Error(ex.Message, 403));
        }
    }

    /// <summary>
    /// 删除评论
    /// </summary>
    /// <param name="id">评论ID</param>
    /// <returns>删除结果</returns>
    /// <response code="200">评论删除成功</response>
    /// <response code="401">未授权访问</response>
    /// <response code="403">无权限删除该评论</response>
    /// <response code="404">评论不存在</response>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(long id)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 删除评论 {CommentId}", userId, id);
        
        try
        {
            var result = await _commentService.DeleteCommentAsync(userId, id);
            
            if (!result)
            {
                return NotFound(ApiResponse<bool>.Error("评论不存在", 404));
            }
            
            return Ok(ApiResponse<bool>.Ok(true, "评论删除成功"));
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "用户 {UserId} 无权限删除评论 {CommentId}", userId, id);
            return StatusCode(403, ApiResponse<bool>.Error(ex.Message, 403));
        }
    }
}
