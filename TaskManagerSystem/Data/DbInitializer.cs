using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskStatus = TaskManagerSystem.Models.TaskStatus;
using TaskItem = TaskManagerSystem.Models.TaskItem;
using User = TaskManagerSystem.Models.User;

namespace TaskManagerSystem.Data;

public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        
        await context.Database.MigrateAsync();
        
        await SeedDataAsync(context);
    }

    private static async Task SeedDataAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
        {
            return;
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123456");

        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@example.com",
            PasswordHash = passwordHash,
            CreatedAt = DateTime.UtcNow
        };

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        var sampleTasks = new List<TaskItem>
        {
            new TaskItem
            {
                Title = "完成项目设计",
                Description = "完成任务管理系统的整体架构设计",
                Status = TaskStatus.Pending,
                UserId = adminUser.Id,
                DueDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Title = "编写API文档",
                Description = "为所有API接口编写详细文档",
                Status = TaskStatus.InProgress,
                UserId = adminUser.Id,
                DueDate = DateTime.UtcNow.AddDays(5),
                CreatedAt = DateTime.UtcNow
            },
            new TaskItem
            {
                Title = "单元测试",
                Description = "编写核心业务逻辑的单元测试",
                Status = TaskStatus.Completed,
                UserId = adminUser.Id,
                DueDate = DateTime.UtcNow.AddDays(-2),
                CreatedAt = DateTime.UtcNow.AddDays(-10)
            }
        };

        await context.Tasks.AddRangeAsync(sampleTasks);
        await context.SaveChangesAsync();
    }
}
