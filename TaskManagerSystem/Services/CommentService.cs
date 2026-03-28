using TaskManagerSystem.Dtos;
using TaskManagerSystem.Models;
using TaskManagerSystem.Repositories;

namespace TaskManagerSystem.Services;

public class CommentService : ICommentService
{
    private readonly IRepository<Comment> _commentRepository;
    private readonly IRepository<TaskItem> _taskRepository;

    public CommentService(IRepository<Comment> commentRepository, IRepository<TaskItem> taskRepository)
    {
        _commentRepository = commentRepository;
        _taskRepository = taskRepository;
    }

    public async Task<CommentResponseDto> CreateCommentAsync(long userId, CreateCommentDto createCommentDto)
    {
        var taskExists = await _taskRepository.ExistsAsync(t => t.Id == createCommentDto.TaskId && t.UserId == userId);
        if (!taskExists)
        {
            throw new KeyNotFoundException("任务不存在或您无权访问该任务");
        }

        var comment = new Comment
        {
            Content = createCommentDto.Content,
            TaskId = createCommentDto.TaskId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var createdComment = await _commentRepository.AddAsync(comment);
        return MapToCommentResponseDto(createdComment);
    }

    public async Task<CommentResponseDto?> GetCommentByIdAsync(long userId, long commentId)
    {
        var comments = await _commentRepository.FindAsync(c => c.Id == commentId && c.Task.UserId == userId);
        var comment = comments.FirstOrDefault();
        return comment == null ? null : MapToCommentResponseDto(comment);
    }

    public async Task<PagedResponseDto<CommentResponseDto>> GetCommentsAsync(long userId, CommentPagedRequestDto request)
    {
        var taskExists = await _taskRepository.ExistsAsync(t => t.Id == request.TaskId && t.UserId == userId);
        if (!taskExists)
        {
            throw new KeyNotFoundException("任务不存在或您无权访问该任务");
        }

        var predicate = PredicateBuilder.True<Comment>();
        predicate = predicate.And(c => c.TaskId == request.TaskId);

        var totalCount = await _commentRepository.CountAsync(predicate);
        var comments = await _commentRepository.GetPagedAsync(request.PageIndex, request.PageSize, predicate);

        return new PagedResponseDto<CommentResponseDto>
        {
            Items = comments.Select(MapToCommentResponseDto).ToList(),
            TotalCount = totalCount,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<bool> DeleteCommentAsync(long userId, long commentId)
    {
        var comments = await _commentRepository.FindAsync(c => c.Id == commentId && c.Task.UserId == userId);
        var comment = comments.FirstOrDefault();

        if (comment == null)
        {
            return false;
        }

        await _commentRepository.DeleteAsync(comment);
        return true;
    }

    private CommentResponseDto MapToCommentResponseDto(Comment comment)
    {
        return new CommentResponseDto
        {
            Id = comment.Id,
            Content = comment.Content,
            TaskId = comment.TaskId,
            UserId = comment.UserId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt
        };
    }
}