using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskManagerSystem.Models.TaskStatus;

namespace TaskManagerSystem.Dtos;

public class CreateTaskDto
{
    [Required(ErrorMessage = "任务标题不能为空")]
    [MaxLength(200, ErrorMessage = "任务标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "任务描述长度不能超过1000个字符")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }
}

public class UpdateTaskDto
{
    [Required(ErrorMessage = "任务标题不能为空")]
    [MaxLength(200, ErrorMessage = "任务标题长度不能超过200个字符")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "任务描述长度不能超过1000个字符")]
    public string? Description { get; set; }

    public TaskStatus Status { get; set; }

    public DateTime? DueDate { get; set; }
}

public class TaskResponseDto
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TaskPagedRequestDto
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 10;
    public TaskStatus? Status { get; set; }
}

public class PagedResponseDto<T>
{
    public List<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
