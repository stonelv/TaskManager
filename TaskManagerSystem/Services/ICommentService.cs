using TaskManagerSystem.Dtos;

namespace TaskManagerSystem.Services;

public interface ICommentService
{
    Task<CommentResponseDto> CreateCommentAsync(long userId, CreateCommentDto createCommentDto);
    Task<CommentResponseDto?> GetCommentByIdAsync(long userId, long commentId);
    Task<PagedResponseDto<CommentResponseDto>> GetCommentsAsync(long userId, CommentPagedRequestDto request);
    Task<bool> DeleteCommentAsync(long userId, long commentId);
}