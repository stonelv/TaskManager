using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TaskManager.API.Models;

namespace TaskManager.API.Filters
{
    public class ValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        k => k.Key,
                        v => v.Value?.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var errorMessage = string.Join("; ", errors
                    .SelectMany(kv => kv.Value ?? Enumerable.Empty<string>()));

                context.Result = new BadRequestObjectResult(ApiResponse.ErrorResult(errorMessage, 400));
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
