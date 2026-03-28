using Microsoft.EntityFrameworkCore;
using TaskManagerSystem.Dtos;
using TaskManagerSystem.Models;
using TaskManagerSystem.Repositories;

namespace TaskManagerSystem.Services;

public class TaskCommentService : ITaskCommentService
{
    private readonly IRepository<TaskComment> _commentRepository;
    private readonly IRepository<TaskItem> _taskRepository;
    private readonly IRepository<User> _userRepository;

    public TaskCommentService(
        IRepository<TaskComment> commentRepository,
        IRepository<TaskItem> taskRepository,
        IRepository<User> userRepository)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    public async Task<CommentResponseDto> CreateCommentAsync(long userId, long taskId, CreateCommentDto createCommentDto)
    {
        // 检查任务是否存在且属于当前用户
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
        {
            throw new KeyNotFoundException("任务不存在或您无权访问此任务");
        }

        var comment = new TaskComment
        {
            Content = createCommentDto.Content,
            TaskId = taskId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createdComment = await _commentRepository.AddAsync(comment);
        
        // 加载用户信息
        var user = await _userRepository.GetByIdAsync(userId);
        
        return MapToCommentResponseDto(createdComment, user?.Username ?? string.Empty);
    }

    public async Task<PagedResponseDto<CommentResponseDto>> GetCommentsByTaskIdAsync(long userId, long taskId, CommentPagedRequestDto request)
    {
        // 检查任务是否存在且属于当前用户
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
        {
            throw new KeyNotFoundException("任务不存在或您无权访问此任务");
        }

        var predicate = PredicateBuilder.True<TaskComment>();
        predicate = predicate.And(c => c.TaskId == taskId);

        var totalCount = await _commentRepository.CountAsync(predicate);
        
        // 使用Include加载用户信息
        var comments = await _commentRepository.GetQueryable()
            .Include(c => c.User)
            .Where(predicate)
            .OrderByDescending(c => c.CreatedAt)
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return new PagedResponseDto<CommentResponseDto>
        {
            Items = comments.Select(c => MapToCommentResponseDto(c, c.User?.Username ?? string.Empty)).ToList(),
            TotalCount = totalCount,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<bool> DeleteCommentAsync(long userId, long taskId, long commentId)
    {
        // 检查任务是否存在且属于当前用户
        var task = await _taskRepository.GetByIdAsync(taskId);
        if (task == null || task.UserId != userId)
        {
            throw new KeyNotFoundException("任务不存在或您无权访问此任务");
        }

        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null || comment.TaskId != taskId)
        {
            return false;
        }

        await _commentRepository.DeleteAsync(comment);
        return true;
    }

    private CommentResponseDto MapToCommentResponseDto(TaskComment comment, string username)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            Username = username,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}