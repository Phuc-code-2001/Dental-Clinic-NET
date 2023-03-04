using DataLayer.DataContexts;
using DataLayer.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.CustomPolicy
{
    public class UserLockMiddleware
    {

        public RequestDelegate _next;
        public AppDbContext _dbContext;

        public UserLockMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        private async Task<bool> UserLockHandlerAsync(HttpContext context)
        {
            

            BaseUser user = await _dbContext.Users
                .Include(_user => _user.UserLocks)
                .FirstOrDefaultAsync(_user => _user.UserName == context.User.Identity.Name);
            UserLock userLock = user.UserLocks.OrderBy(l => l.TimeCreated).LastOrDefault();

            return userLock != null && userLock.IsLockCalculated;
        }

        public async Task Invoke(HttpContext context)
        {
            var authenResult = await context.AuthenticateAsync();
            if (authenResult.Succeeded)
            {
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
