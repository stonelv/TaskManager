using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskStatus = TaskManager.API.Models.TaskStatus;

namespace TaskManager.API.Services
{
    public interface ITaskService : IService<TaskItem>
    {
        Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, Guid userId);
        Task<TaskDto> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateTaskDto, Guid userId);
        Task DeleteTaskAsync(Guid taskId, Guid userId);
        Task<TaskDto> GetTaskByIdAsync(Guid taskId, Guid userId);
        Task<PagedResultDto<TaskDto>> GetUserTasksAsync(Guid userId, int pageNumber = 1, int pageSize = 10, TaskStatus? status = null);
    }
}
