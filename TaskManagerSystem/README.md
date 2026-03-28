# 任务管理系统 Web API

基于 .NET 10 的任务管理系统纯后端项目，采用 ASP.NET Core Web API + EF Core + SQLite 技术栈。

## 项目结构

```
TaskManagerSystem/
├── Controllers/          # 控制器层
│   ├── AuthController.cs    # 认证控制器（注册/登录）
│   └── TaskController.cs    # 任务控制器（CRUD）
├── Models/               # 数据模型层
│   ├── User.cs              # 用户实体
│   └── TaskItem.cs          # 任务实体
├── Dtos/                 # 数据传输对象
│   ├── UserDtos.cs          # 用户相关DTO
│   └── TaskDtos.cs          # 任务相关DTO
├── Services/             # 业务逻辑层
│   ├── IUserService.cs      # 用户服务接口
│   ├── UserService.cs       # 用户服务实现
│   ├── ITaskService.cs      # 任务服务接口
│   └── TaskService.cs       # 任务服务实现
├── Repositories/         # 数据访问层
│   ├── IRepository.cs       # 泛型仓储接口
│   └── Repository.cs        # 泛型仓储实现
├── Data/                 # 数据层
│   ├── AppDbContext.cs      # 数据库上下文
│   ├── AppDbContextFactory.cs # 设计时工厂
│   └── DbInitializer.cs     # 数据库初始化器
├── Common/               # 公共类
│   ├── ApiResponse.cs       # 统一返回格式
│   └── JwtSettings.cs       # JWT配置
├── Filters/              # 过滤器
│   ├── GlobalExceptionFilter.cs # 全局异常过滤器
│   └── ValidationFilter.cs     # 参数验证过滤器
├── Program.cs            # 程序入口
├── appsettings.json      # 配置文件
└── TaskManagerSystem.csproj # 项目文件
```

## 关键设计说明

### 1. 分层架构
- **Controller层**：处理HTTP请求，参数校验，返回统一格式结果
- **Service层**：封装业务逻辑，实现核心功能
- **Repository层**：封装数据访问，提供通用CRUD操作
- **Model层**：定义数据实体
- **DTO层**：定义数据传输对象，隔离内外层

### 2. 安全认证
- 使用JWT（JSON Web Token）进行身份认证
- 密码使用BCrypt加密存储
- 支持用户名/邮箱登录

### 3. 统一返回格式
所有API接口返回统一格式：
```json
{
  "success": true,
  "message": "操作成功",
  "data": {},
  "code": 200
}
```

### 4. 全局异常处理
- 使用GlobalExceptionFilter捕获所有未处理异常
- 统一返回500错误格式
- 记录异常日志

### 5. 参数校验
- 使用DataAnnotations进行参数验证
- ValidationFilter自动处理验证失败情况
- 返回400错误

### 6. 日志记录
- 使用Serilog记录日志
- 支持控制台和文件输出
- 按天滚动日志文件

### 7. 数据库
- 使用SQLite轻量级数据库
- 支持自动迁移
- 应用启动时自动初始化种子数据

## 启动步骤

### 1. 环境要求
- .NET 10 SDK
- 任意代码编辑器（VS Code / Visual Studio / Rider）

### 2. 恢复依赖
```bash
cd TaskManagerSystem
dotnet restore
```

### 3. 生成数据库迁移（可选，已包含初始迁移）
```bash
dotnet ef migrations add InitialCreate
```

### 4. 运行项目
```bash
dotnet run
```

项目将在 `http://localhost:5000` 和 `https://localhost:5001` 启动

### 5. 访问Swagger文档
打开浏览器访问：`https://localhost:5001/swagger`

## 接口测试示例

### 1. 用户注册

**请求：**
```bash
POST https://localhost:5001/api/auth/register
Content-Type: application/json

{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test@123456"
}
```

**响应：**
```json
{
  "success": true,
  "message": "注册成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 2,
      "username": "testuser",
      "email": "test@example.com",
      "createdAt": "2024-01-01T00:00:00Z"
    },
    "expiresAt": "2024-01-01T01:00:00Z"
  },
  "code": 200
}
```

### 2. 用户登录

**请求：**
```bash
POST https://localhost:5001/api/auth/login
Content-Type: application/json

{
  "usernameOrEmail": "admin",
  "password": "Admin@123456"
}
```

**响应：**
```json
{
  "success": true,
  "message": "登录成功",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "user": {
      "id": 1,
      "username": "admin",
      "email": "admin@example.com",
      "createdAt": "2024-01-01T00:00:00Z"
    },
    "expiresAt": "2024-01-01T01:00:00Z"
  },
  "code": 200
}
```

### 3. 创建任务

**请求：**
```bash
POST https://localhost:5001/api/task
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{
  "title": "学习.NET 10",
  "description": "完成.NET 10 Web API开发学习",
  "dueDate": "2024-12-31T23:59:59Z"
}
```

**响应：**
```json
{
  "success": true,
  "message": "任务创建成功",
  "data": {
    "id": 4,
    "title": "学习.NET 10",
    "description": "完成.NET 10 Web API开发学习",
    "status": 0,
    "dueDate": "2024-12-31T23:59:59Z",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": null
  },
  "code": 200
}
```

### 4. 获取任务列表（分页+筛选）

**请求：**
```bash
GET https://localhost:5001/api/task?pageIndex=0&pageSize=10&status=1
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**响应：**
```json
{
  "success": true,
  "message": "获取任务列表成功",
  "data": {
    "items": [
      {
        "id": 2,
        "title": "编写API文档",
        "description": "为所有API接口编写详细文档",
        "status": 1,
        "dueDate": "2024-01-06T00:00:00Z",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": null
      }
    ],
    "totalCount": 1,
    "pageIndex": 0,
    "pageSize": 10,
    "totalPages": 1
  },
  "code": 200
}
```

### 5. 更新任务

**请求：**
```bash
PUT https://localhost:5001/api/task/1
Content-Type: application/json
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...

{
  "title": "完成项目设计（已更新）",
  "description": "完成任务管理系统的整体架构设计",
  "status": 1,
  "dueDate": "2024-01-10T00:00:00Z"
}
```

**响应：**
```json
{
  "success": true,
  "message": "任务更新成功",
  "data": {
    "id": 1,
    "title": "完成项目设计（已更新）",
    "description": "完成任务管理系统的整体架构设计",
    "status": 1,
    "dueDate": "2024-01-10T00:00:00Z",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:05:00Z"
  },
  "code": 200
}
```

## 默认账号

系统初始化时会创建一个默认管理员账号：
- 用户名：`admin`
- 邮箱：`admin@example.com`
- 密码：`Admin@123456`

## 任务状态说明

- `0` - 待处理（Pending）
- `1` - 进行中（InProgress）
- `2` - 已完成（Completed）
