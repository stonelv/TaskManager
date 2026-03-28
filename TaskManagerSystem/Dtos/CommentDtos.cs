using System.ComponentModel.DataAnnotations;

namespace TaskManagerSystem.Dtos;

public class CreateCommentDto
{
    [Required(ErrorMessage = "评论内容不能为空")]
    [MaxLength(1000, ErrorMessage = "评论内容长度不能超过1000个字符")]
    public string Content { get; set; } = string.Empty;

    [Required(ErrorMessage = "任务ID不能为空")]
    public long TaskId { get; set; }
}

public class CommentResponseDto
{
    public long Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public long TaskId { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CommentPagedRequestDto
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;

    [Required(ErrorMessage = "任务ID不能为空")]
    public long TaskId { get; set; }
}