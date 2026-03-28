using TaskManagerSystem.Dtos;
using TaskStatus = TaskManagerSystem.Models.TaskStatus;
using TaskItem = TaskManagerSystem.Models.TaskItem;
using TaskManagerSystem.Repositories;

namespace TaskManagerSystem.Services;

public class TaskService : ITaskService
{
    private readonly IRepository<TaskItem> _taskRepository;

    public TaskService(IRepository<TaskItem> taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskResponseDto> CreateTaskAsync(long userId, CreateTaskDto createTaskDto)
    {
        var task = new TaskItem
        {
            Title = createTaskDto.Title,
            Description = createTaskDto.Description,
            DueDate = createTaskDto.DueDate,
            UserId = userId,
            Status = TaskStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        var createdTask = await _taskRepository.AddAsync(task);
        return MapToTaskResponseDto(createdTask);
    }

    public async Task<TaskResponseDto?> GetTaskByIdAsync(long userId, long taskId)
    {
        var tasks = await _taskRepository.FindAsync(t => t.Id == taskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();
        return task == null ? null : MapToTaskResponseDto(task);
    }

    public async Task<PagedResponseDto<TaskResponseDto>> GetTasksAsync(long userId, TaskPagedRequestDto request)
    {
        var predicate = PredicateBuilder.True<TaskItem>();
        predicate = predicate.And(t => t.UserId == userId);

        if (request.Status.HasValue)
        {
            predicate = predicate.And(t => t.Status == request.Status.Value);
        }

        var totalCount = await _taskRepository.CountAsync(predicate);
        var tasks = await _taskRepository.GetPagedAsync(request.PageIndex, request.PageSize, predicate);

        return new PagedResponseDto<TaskResponseDto>
        {
            Items = tasks.Select(MapToTaskResponseDto).ToList(),
            TotalCount = totalCount,
            PageIndex = request.PageIndex,
            PageSize = request.PageSize
        };
    }

    public async Task<TaskResponseDto?> UpdateTaskAsync(long userId, long taskId, UpdateTaskDto updateTaskDto)
    {
        var tasks = await _taskRepository.FindAsync(t => t.Id == taskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();

        if (task == null)
        {
            return null;
        }

        task.Title = updateTaskDto.Title;
        task.Description = updateTaskDto.Description;
        task.Status = updateTaskDto.Status;
        task.DueDate = updateTaskDto.DueDate;
        task.UpdatedAt = DateTime.UtcNow;

        var updatedTask = await _taskRepository.UpdateAsync(task);
        return MapToTaskResponseDto(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(long userId, long taskId)
    {
        var tasks = await _taskRepository.FindAsync(t => t.Id == taskId && t.UserId == userId);
        var task = tasks.FirstOrDefault();

        if (task == null)
        {
            return false;
        }

        await _taskRepository.DeleteAsync(task);
        return true;
    }

    private TaskResponseDto MapToTaskResponseDto(TaskItem task)
    {
        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            Status = task.Status,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }
}

public static class PredicateBuilder
{
    public static System.Linq.Expressions.Expression<Func<T, bool>> True<T>() { return f => true; }
    public static System.Linq.Expressions.Expression<Func<T, bool>> False<T>() { return f => false; }

    public static System.Linq.Expressions.Expression<Func<T, bool>> And<T>(
        this System.Linq.Expressions.Expression<Func<T, bool>> expr1,
        System.Linq.Expressions.Expression<Func<T, bool>> expr2)
    {
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(T));
        var body = System.Linq.Expressions.Expression.AndAlso(
            System.Linq.Expressions.Expression.Invoke(expr1, parameter),
            System.Linq.Expressions.Expression.Invoke(expr2, parameter));
        return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(body, parameter);
    }
}
