using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using TaskManager.API.Exceptions;
using TaskManager.API.Filters;
using TaskManager.API.Models;
using TaskManager.API.Repositories;
using TaskManager.API.Services;
using TaskStatus = TaskManager.API.Models.TaskStatus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationFilter>();
});

// Configure DbContext with SQLite
builder.Services.AddDbContext<TaskManagerContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };
});

// Configure Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskManager API", Version = "v1" });
    
    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Register Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManager API v1");
    });
}

app.UseHttpsRedirection();

// Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Auto Migration and Seed Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<TaskManagerContext>();
    
    // Ensure database is created and migrations are applied
    context.Database.EnsureCreated();
    
    // Seed data if needed
    if (!context.Users.Any())
    {
        var authService = services.GetRequiredService<IAuthService>();
        
        var seedUser = new User
        {
            Id = Guid.NewGuid(),
            Username = "admin",
            Email = "admin@taskmanager.com",
            PasswordHash = authService.HashPassword("Admin@123"),
            CreatedAt = DateTime.UtcNow
        };
        
        context.Users.Add(seedUser);
        context.SaveChanges();
        
        // Seed some tasks for the admin user
        var seedTasks = new List<TaskItem>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Title = "完成项目需求分析",
                Description = "分析任务管理系统的需求，确定功能模块",
                Status = TaskStatus.Completed,
                UserId = seedUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "设计数据库结构",
                Description = "设计用户表和任务表的结构",
                Status = TaskStatus.Completed,
                UserId = seedUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "实现用户认证模块",
                Description = "实现JWT用户注册和登录功能",
                Status = TaskStatus.Completed,
                UserId = seedUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "实现任务管理功能",
                Description = "实现任务的增删改查、分页和筛选功能",
                Status = TaskStatus.InProgress,
                UserId = seedUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new()
            {
                Id = Guid.NewGuid(),
                Title = "编写API文档",
                Description = "完成Swagger文档配置和接口测试",
                Status = TaskStatus.Pending,
                UserId = seedUser.Id,
                CreatedAt = DateTime.UtcNow
            }
        };
        
        context.Tasks.AddRange(seedTasks);
        context.SaveChanges();
    }
}

app.Run();
