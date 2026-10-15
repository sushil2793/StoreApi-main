using Microsoft.AspNetCore.Http;
using SmartMeterAdmin.Infrastructure.Helpers;
using SmartMeterAdmin.Services.User;
using System.Threading.Tasks;

namespace SmartMeterAdmin.Middleware
{
    public class RequestAccessMiddleware
    {
        private readonly RequestDelegate next;
        public RequestAccessMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        public async Task InvokeAsync(HttpContext context, IUser user)
        {
            var userContext = context.User;
            if (userContext.Identity?.IsAuthenticated == true)
            {
                var isSuspended = await user.IsSuspendedUser(userContext.Identity.GetUserId());
                if (isSuspended)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync("Forbidden: Your account is suspended.");
                    return;
                }
            }
            await next(context);
        }
    }
}
