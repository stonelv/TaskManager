using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using TaskManagerSystem.Common;

namespace TaskManagerSystem.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var exception = context.Exception;
        _logger.LogError(exception, "发生未处理的异常");

        var result = new ObjectResult(ApiResponse.Error(exception.Message, 500))
        {
            StatusCode = 500
        };

        context.Result = result;
        context.ExceptionHandled = true;
    }
}
