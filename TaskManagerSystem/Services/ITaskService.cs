using TaskManagerSystem.Dtos;
using TaskManagerSystem.Models;

namespace TaskManagerSystem.Services;

public interface ITaskService
{
    Task<TaskResponseDto> CreateTaskAsync(long userId, CreateTaskDto createTaskDto);
    Task<TaskResponseDto?> GetTaskByIdAsync(long userId, long taskId);
    Task<PagedResponseDto<TaskResponseDto>> GetTasksAsync(long userId, TaskPagedRequestDto request);
    Task<TaskResponseDto?> UpdateTaskAsync(long userId, long taskId, UpdateTaskDto updateTaskDto);
    Task<bool> DeleteTaskAsync(long userId, long taskId);
}
