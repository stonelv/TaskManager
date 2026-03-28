namespace TaskManager.API.Models
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int Code { get; set; }

        protected ApiResponse(bool success, string message, int code)
        {
            Success = success;
            Message = message;
            Code = code;
        }

        public static ApiResponse SuccessResult(string message = "操作成功")
        {
            return new ApiResponse(true, message, 200);
        }

        public static ApiResponse ErrorResult(string message = "操作失败", int code = 400)
        {
            return new ApiResponse(false, message, code);
        }
    }

    public class ApiResponse<T> : ApiResponse
    {
        public T? Data { get; set; }

        private ApiResponse(bool success, string message, int code, T? data)
            : base(success, message, code)
        {
            Data = data;
        }

        public static ApiResponse<T> SuccessResult(T data, string message = "操作成功")
        {
            return new ApiResponse<T>(true, message, 200, data);
        }

        public new static ApiResponse<T> ErrorResult(string message = "操作失败", int code = 400)
        {
            return new ApiResponse<T>(false, message, code, default);
        }
    }
}
