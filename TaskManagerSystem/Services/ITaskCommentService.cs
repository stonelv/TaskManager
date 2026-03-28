using TaskManagerSystem.Dtos;
using TaskManagerSystem.Models;

namespace TaskManagerSystem.Services;

public interface ITaskCommentService
{
    Task<CommentResponseDto> CreateCommentAsync(long userId, long taskId, CreateCommentDto createCommentDto);
    Task<PagedResponseDto<CommentResponseDto>> GetCommentsByTaskIdAsync(long userId, long taskId, CommentPagedRequestDto request);
    Task<bool> DeleteCommentAsync(long userId, long taskId, long commentId);
}