using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequiresBearerTokenAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ActionArguments.TryGetValue("bearerToken", out var value) ||
                value is not string token ||
                string.IsNullOrWhiteSpace(token))
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Success = false,
                    Message = "Missing Bearer token. Please authenticate first."
                });
            }
        }
    }
}
