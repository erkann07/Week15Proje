using TuneStore.Application.Abstractions;
using TuneStore.Domain.Entities;

namespace TuneStore.Api.Middlewares
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public RequestLoggingMiddleware(RequestDelegate next) => _next = next;

        public async Task Invoke(HttpContext context, IUnitOfWork uow)
        {
            var log = new RequestLog
            {
                RequestedAtUtc = DateTime.UtcNow,
                Path = context.Request.Path,
                Method = context.Request.Method,
                Ip = context.Connection.RemoteIpAddress?.ToString()
            };

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var idClaim = context.User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier || c.Type == "sub");
                if (int.TryParse(idClaim?.Value, out var uid)) log.UserId = uid;
            }

            await uow.RequestLogs.AddAsync(log);
            await uow.SaveChangesAsync();
            await _next(context);
        }
    }
}

