using Microsoft.EntityFrameworkCore;
using TaskManagerSystem.Dtos;
using TaskManagerSystem.Models;
using TaskManagerSystem.Repositories;

namespace TaskManagerSystem.Services;

public class CommentService : ICommentService
{
    private readonly IRepository<TaskComment> _commentRepository;
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<User> _userRepository;

    public CommentService(
        IRepository<TaskComment> commentRepository,
        IRepository<TaskItem> taskRepository,
        IRepository<User> userRepository)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<CommentResponseDto> CreateCommentAsync(long userId, CreateCommentDto createCommentDto)
    {
        var tasks = await _taskRepository.FindAsync(t => t.Id == createCommentDto.TaskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();

        if (task == null)
        {
            throw new KeyNotFoundException("任务不存在或无权限操作");
        }

        var comment = new TaskComment
        {
            TaskId = createCommentDto.TaskId,
            UserId = userId,
            Content = createCommentDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        var createdComment = await _commentRepository.AddAsync(comment);
        
        var users = await _userRepository.FindAsync(u => u.Id == userId);
        var user = users.FirstOrDefault();
        
        return MapToCommentResponseDto(createdComment, user?.Username ?? "未知用户");
    }

    public async Task<PagedResponseDto<CommentResponseDto>> GetCommentsAsync(long userId, CommentPagedRequestDto request)
    {
        var tasks = await _taskRepository.FindAsync(t => t.Id == request.TaskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();

        if (task == null)
        {
            throw new KeyNotFoundException("任务不存在或无权限操作");
        }

        var predicate = PredicateBuilder.True<TaskComment>();
        predicate = predicate.And(c => c.TaskId == request.TaskId);

        var totalCount = await _commentRepository.CountAsync(predicate);
        var comments = await _commentRepository.GetPagedAsync(request.PageIndex, request.PageSize, predicate);

        var userIds = comments.Select(c => c.UserId).Distinct().ToList();
        var users = await _userRepository.FindAsync(u => userIds.Contains(u.Id));
        var userDict = users.ToDictionary(u => u.Id, u => u.Username);

        return new PagedResponseDto<CommentResponseDto>
        {
            Items = comments.Select(c => MapToCommentResponseDto(c, userDict.TryGetValue(c.UserId, out var username) ? username : "未知用户")).ToList(),
            TotalCount = totalCount,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<bool> DeleteCommentAsync(long userId, long commentId)
    {
        var comments = await _commentRepository.FindAsync(c => c.Id == commentId);
        var comment = comments.FirstOrDefault();

        if (comment == null)
        {
            return false;
        }

        var tasks = await _taskRepository.FindAsync(t => t.Id == comment.TaskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();

        if (task == null)
        {
            throw new UnauthorizedAccessException("无权限删除该评论");
        }

        await _commentRepository.DeleteAsync(comment);
        return true;
    }

    public async Task<CommentResponseDto?> GetCommentByIdAsync(long userId, long commentId)
    {
        var comments = await _commentRepository.FindAsync(c => c.Id == commentId);
        var comment = comments.FirstOrDefault();

        if (comment == null)
        {
            return null;
        }

        var tasks = await _taskRepository.FindAsync(t => t.Id == comment.TaskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();

        if (task == null)
        {
            throw new UnauthorizedAccessException("无权限查看该评论");
        }

        var users = await _userRepository.FindAsync(u => u.Id == comment.UserId);
        var user = users.FirstOrDefault();

        return MapToCommentResponseDto(comment, user?.Username ?? "未知用户");
    }

    private CommentResponseDto MapToCommentResponseDto(TaskComment comment, string username)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Username = username
        };
    }
}
