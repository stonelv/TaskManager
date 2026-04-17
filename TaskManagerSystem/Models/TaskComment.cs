using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagerSystem.Models;

public class TaskComment
{
    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("task_id")]
    public long TaskId { get; set; }

    [Required]
    [Column("user_id")]
    public long UserId { get; set; }

    [Required]
    [Column("content")]
    [MaxLength(1000, ErrorMessage = "评论内容长度不能超过1000个字符")]
    public string Content { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey(nameof(TaskId))]
    public virtual TaskItem Task { get; set; } = null!;

    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
