using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TuneStore.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class TimeWindowFilterAttribute : Attribute, IAsyncActionFilter
    {
        public int StartHourUtc { get; }
        public int EndHourUtc { get; }
        public TimeWindowFilterAttribute(int startHourUtc, int endHourUtc)
        { StartHourUtc = startHourUtc; EndHourUtc = endHourUtc; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var hour = DateTime.UtcNow.Hour;
            var ok = StartHourUtc <= EndHourUtc
                ? (hour >= StartHourUtc && hour < EndHourUtc)
                : (hour >= StartHourUtc || hour < EndHourUtc);

            if (!ok)
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status423Locked,
                    Content = $"This endpoint is accessible between {StartHourUtc}:00-{EndHourUtc}:00 UTC"
                };
                return;
            }
            await next();
        }
    }
}
