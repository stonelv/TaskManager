using System.Linq.Expressions;
using TaskManager.API.DTOs;
using TaskManager.API.Models;
using TaskManager.API.Repositories;
using TaskStatus = TaskManager.API.Models.TaskStatus;

namespace TaskManager.API.Services
{
    public class TaskService : Service<TaskItem>, ITaskService
    {
        private readonly IRepository<TaskItem> _taskRepository;

        public TaskService(IRepository<TaskItem> taskRepository) 
            : base(taskRepository)
        {
            _taskRepository = taskRepository;
        }

        public async Task<TaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, Guid userId)
        {
            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = createTaskDto.Title,
                Description = createTaskDto.Description,
                Status = createTaskDto.Status,
                DueDate = createTaskDto.DueDate,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _taskRepository.AddAsync(task);

            return MapToDto(task);
        }

        public async Task<TaskDto> UpdateTaskAsync(Guid taskId, UpdateTaskDto updateTaskDto, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("您没有权限修改此任务");
            }

            task.Title = updateTaskDto.Title;
            task.Description = updateTaskDto.Description;
            task.Status = updateTaskDto.Status;
            task.DueDate = updateTaskDto.DueDate;
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return MapToDto(task);
        }

        public async Task DeleteTaskAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("您没有权限删除此任务");
            }

            await _taskRepository.DeleteAsync(task);
        }

        public async Task<TaskDto> GetTaskByIdAsync(Guid taskId, Guid userId)
        {
            var task = await _taskRepository.GetByIdAsync(taskId);
            
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("您没有权限查看此任务");
            }

            return MapToDto(task);
        }

        public async Task<PagedResultDto<TaskDto>> GetUserTasksAsync(Guid userId, int pageNumber = 1, int pageSize = 10, TaskStatus? status = null)
        {
            Expression<Func<TaskItem, bool>> predicate = t => t.UserId == userId;
            
            if (status.HasValue)
            {
                predicate = t => t.UserId == userId && t.Status == status.Value;
            }

            var (items, totalCount) = await GetPagedAsync(pageNumber, pageSize, predicate);
            
            var taskDtos = items.Select(MapToDto);

            return new PagedResultDto<TaskDto>
            {
                Items = taskDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private static TaskDto MapToDto(TaskItem task)
        {
            return new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                DueDate = task.DueDate,
                UserId = task.UserId,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}
