# 任务评论模块 API 接口示例

## 前提条件
- 已通过身份验证（JWT Token）
- 已有任务存在（任务ID通常通过任务列表接口获取）

---

## 1. 创建任务评论

**请求示例：**
```http
POST /api/tasks/1/comments
Content-Type: application/json
Authorization: Bearer {your-jwt-token}

{
  "content": "这个任务需要更多的信息，能否补充一下具体的需求？"
}
```

**响应示例（成功）：**
```json
{
  "success": true,
  "message": "评论创建成功",
  "data": {
    "id": 1,
    "content": "这个任务需要更多的信息，能否补充一下具体的需求？",
    "taskId": 1,
    "userId": 1,
    "username": "admin",
    "createdAt": "2026-03-28T05:15:23.1234567Z",
    "updatedAt": null
  },
  "code": 200
}
```

**响应示例（失败 - 任务不存在）：**
```json
{
  "success": false,
  "message": "任务不存在或您无权访问此任务",
  "data": null,
  "code": 404
}
```

---

## 2. 获取任务评论列表（分页）

**请求示例：**
```http
GET /api/tasks/1/comments?pageIndex=0&pageSize=10
Authorization: Bearer {your-jwt-token}
```

**响应示例（成功）：**
```json
{
  "success": true,
  "message": "获取评论列表成功",
  "data": {
    "items": [
      {
        "id": 1,
        "content": "这个任务需要更多的信息，能否补充一下具体的需求？",
        "taskId": 1,
        "userId": 1,
        "username": "admin",
        "createdAt": "2026-03-28T05:15:23.1234567Z",
        "updatedAt": null
      },
      {
        "id": 2,
        "content": "好的，我会补充详细的需求文档。",
        "taskId": 1,
        "userId": 1,
        "username": "admin",
        "createdAt": "2026-03-28T05:20:15.9876543Z",
        "updatedAt": null
      }
    ],
    "totalCount": 2,
    "pageIndex": 0,
    "pageSize": 10,
    "totalPages": 1
  },
  "code": 200
}
```

---

## 3. 删除任务评论

**请求示例：**
```http
DELETE /api/tasks/1/comments/2
Authorization: Bearer {your-jwt-token}
```

**响应示例（成功）：**
```json
{
  "success": true,
  "message": "评论删除成功",
  "data": true,
  "code": 200
}
```

**响应示例（失败 - 评论不存在）：**
```json
{
  "success": false,
  "message": "评论不存在",
  "data": false,
  "code": 404
}
```

---

## 4. 创建任务评论（验证失败 - 内容为空）

**请求示例：**
```http
POST /api/tasks/1/comments
Content-Type: application/json
Authorization: Bearer {your-jwt-token}

{
  "content": ""
}
```

**响应示例（验证失败）：**
```json
{
  "success": false,
  "message": "请求验证失败",
  "data": {
    "errors": {
      "Content": [
        "评论内容不能为空"
      ]
    }
  },
  "code": 400
}
```

---

## 5. 创建任务评论（验证失败 - 内容过长）

**请求示例：**
```http
POST /api/tasks/1/comments
Content-Type: application/json
Authorization: Bearer {your-jwt-token}

{
  "content": "这是一个非常长的评论..."（超过1000个字符）
}
```

**响应示例（验证失败）：**
```json
{
  "success": false,
  "message": "请求验证失败",
  "data": {
    "errors": {
      "Content": [
        "评论内容长度不能超过1000个字符"
      ]
    }
  },
  "code": 400
}
```

---

## 6. 获取任务评论列表（不同页码）

**请求示例：**
```http
GET /api/tasks/1/comments?pageIndex=1&pageSize=5
Authorization: Bearer {your-jwt-token}
```

**响应示例（成功）：**
```json
{
  "success": true,
  "message": "获取评论列表成功",
  "data": {
    "items": [
      {
        "id": 6,
        "content": "第6条评论内容",
        "taskId": 1,
        "userId": 1,
        "username": "admin",
        "createdAt": "2026-03-28T05:35:00.0000000Z",
        "updatedAt": null
      },
      {
        "id": 7,
        "content": "第7条评论内容",
        "taskId": 1,
        "userId": 1,
        "username": "admin",
        "createdAt": "2026-03-28T05:36:00.0000000Z",
        "updatedAt": null
      }
    ],
    "totalCount": 7,
    "pageIndex": 1,
    "pageSize": 5,
    "totalPages": 2
  },
  "code": 200
}
```

---

## 7. 尝试访问其他用户的任务评论

**请求示例：**
```http
GET /api/tasks/2/comments（任务2属于其他用户）
Authorization: Bearer {your-jwt-token}
```

**响应示例（失败 - 权限不足）：**
```json
{
  "success": false,
  "message": "任务不存在或您无权访问此任务",
  "data": null,
  "code": 404
}
```

---

## 8. 尝试删除其他用户的任务评论

**请求示例：**
```http
DELETE /api/tasks/2/comments/3（评论3属于其他用户的任务）
Authorization: Bearer {your-jwt-token}
```

**响应示例（失败 - 权限不足）：**
```json
{
  "success": false,
  "message": "任务不存在或您无权访问此任务",
  "data": null,
  "code": 404
}
```