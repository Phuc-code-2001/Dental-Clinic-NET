using DataLayer.DataContexts;
using DataLayer.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.CustomPolicy
{
    public class UserLockMiddleware
    {

        public RequestDelegate _next;
        public AppDbContext _dbContext;
        public UserManager<BaseUser> _userManager;

        public UserLockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private async Task<bool> UserLockHandlerAsync(HttpContext context)
        {
            

            BaseUser user = await _userManager.FindByNameAsync(context.User.Identity.Name);
            UserLock userLock = await _dbContext.UserLocks
                .FirstOrDefaultAsync(l => l.BaseUserId == user.Id);

            return userLock != null && userLock.IsLockCalculated;
        }

        public async Task Invoke(HttpContext context)
        {
            var authenResult = await context.AuthenticateAsync();
            if (authenResult.Succeeded)
            {
                _userManager = context.RequestServices.GetService<UserManager<BaseUser>>();
                _dbContext = context.RequestServices.GetService<AppDbContext>();

                var checkUserLockResult = await UserLockHandlerAsync(context);
                if (checkUserLockResult)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return;
                }
            }

            await _next(context);
        }
    }
}
