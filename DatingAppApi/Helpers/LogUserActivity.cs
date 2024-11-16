using DatingAppApi.Controllers;
using DatingAppApi.Extensions;
using DatingAppApi.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace DatingAppApi.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            //after action executed
            if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;

            var userId = context.HttpContext.User.GetUserId();

            var repo = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            var user = await repo.GetUserByIdAsync(userId);
            if (user == null) return;

            user.LastActive = DateTime.UtcNow;

            await repo.SaveAllAsync();

        }
    }
}
