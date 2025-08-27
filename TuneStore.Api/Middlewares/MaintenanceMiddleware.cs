using TuneStore.Application.Abstractions;

namespace TuneStore.Api.Middlewares
{
    public class MaintenanceMiddleware
    {
        private readonly RequestDelegate _next;
        public MaintenanceMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, IUnitOfWork uow)
        {
            var now = DateTime.UtcNow;
            var mw = (await uow.MaintenanceWindows.FindAsync(m => m.IsEnabled && m.StartUtc <= now && now <= m.EndUtc)).FirstOrDefault();
            if (mw != null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsJsonAsync(new { message = mw.Message ?? "Maintenance in progress" });
                return;
            }
            await _next(context);
        }
    }
}
