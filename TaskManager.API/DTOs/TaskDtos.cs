using System.ComponentModel.DataAnnotations;
using TaskStatus = TaskManager.API.Models.TaskStatus;

namespace TaskManager.API.DTOs
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "任务标题不能为空")]
        [MinLength(1, ErrorMessage = "任务标题至少需要1个字符")]
        [MaxLength(100, ErrorMessage = "任务标题不能超过100个字符")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "任务描述不能超过500个字符")]
        public string? Description { get; set; }

        public TaskStatus Status { get; set; } = TaskStatus.Pending;
        public DateTime? DueDate { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required(ErrorMessage = "任务标题不能为空")]
        [MinLength(1, ErrorMessage = "任务标题至少需要1个字符")]
        [MaxLength(100, ErrorMessage = "任务标题不能超过100个字符")]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "任务描述不能超过500个字符")]
        public string? Description { get; set; }

        public TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public TaskStatus Status { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
