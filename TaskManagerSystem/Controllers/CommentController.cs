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
    [HttpPost]
    public async Task<ActionResult<ApiResponse<CommentResponseDto>>> CreateComment([FromBody] CreateCommentDto createCommentDto)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 为任务 {TaskId} 创建评论", userId, createCommentDto.TaskId);

        var result = await _commentService.CreateCommentAsync(userId, createCommentDto);
        return Ok(ApiResponse<CommentResponseDto>.Ok(result, "评论创建成功"));
    }

    /// <summary>
    /// 根据ID获取评论
    /// </summary>
    /// <param name="id">评论ID</param>
    /// <returns>评论详情</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<CommentResponseDto>>> GetCommentById(long id)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取评论 {CommentId}", userId, id);

        var result = await _commentService.GetCommentByIdAsync(userId, id);

        if (result == null)
        {
            return NotFound(ApiResponse<CommentResponseDto>.Error("评论不存在或您无权访问", 404));
        }

        return Ok(ApiResponse<CommentResponseDto>.Ok(result, "获取评论成功"));
    }

    /// <summary>
    /// 获取任务评论列表（支持分页）
    /// </summary>
    /// <param name="request">分页和任务ID参数</param>
    /// <returns>评论列表</returns>
    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResponseDto<CommentResponseDto>>>> GetComments(
        [FromQuery] CommentPagedRequestDto request)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 获取任务 {TaskId} 的评论列表", userId, request.TaskId);

        var result = await _commentService.GetCommentsAsync(userId, request);
        return Ok(ApiResponse<PagedResponseDto<CommentResponseDto>>.Ok(result, "获取评论列表成功"));
    }

    /// <summary>
    /// 删除评论
    /// </summary>
    /// <param name="id">评论ID</param>
    /// <returns>删除结果</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(long id)
    {
        var userId = GetUserId();
        _logger.LogInformation("用户 {UserId} 删除评论 {CommentId}", userId, id);

        var result = await _commentService.DeleteCommentAsync(userId, id);

        if (!result)
        {
            return NotFound(ApiResponse<bool>.Error("评论不存在或您无权访问", 404));
        }

        return Ok(ApiResponse<bool>.Ok(true, "评论删除成功"));
    }
}