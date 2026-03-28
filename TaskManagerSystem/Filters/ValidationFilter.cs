using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManagerSystem.Common;

namespace TaskManagerSystem.Filters;

public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .Select(x => new
                {
                    Field = x.Key,
                    Errors = x.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                })
                .ToList();

            var result = new ObjectResult(ApiResponse.Error("参数验证失败", 400))
            {
                StatusCode = 400
            };

            context.Result = result;
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
