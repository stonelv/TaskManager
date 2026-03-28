namespace TaskManagerSystem.Common;

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public object? Data { get; set; }
    public int Code { get; set; }

    public static ApiResponse Ok(object? data = null, string message = "操作成功")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Data = data,
            Code = 200
        };
    }

    public static ApiResponse Error(string message = "操作失败", int code = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Data = null,
            Code = code
        };
    }
}

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public int Code { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Code = 200
        };
    }

    public static ApiResponse<T> Error(string message = "操作失败", int code = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default,
            Code = code
        };
    }
}
