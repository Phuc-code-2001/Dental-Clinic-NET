using DataLayer.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Dental_Clinic_NET.API.Models.Users;
using Dental_Clinic_NET.API.Services;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        ServicesManager _servicesManager;

        public LoginController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        public async Task<IActionResult> LoginBasicAsync(BasicLoginModel loginModel)
        {
            try
            {
                BaseUser user = await _servicesManager.DbContext.Users
                    .Include(u => u.UserLocks)
                    .FirstOrDefaultAsync(u => u.UserName == loginModel.UserName);

                if (user == null)
                {
                    return Unauthorized("UserName or Password incorrect...");
                }

                UserLock userLock = user.UserLocks.OrderBy(e => e.TimeCreated).LastOrDefault();
                if (userLock != null)
                {
                    if(userLock.IsLockCalculated)
                    {
                        string expired = userLock.Expired.ToString("HH'h'mm dd/MM/yyyy");
                        return Unauthorized($"Your account is lock until {expired}");
                    }
                }

                bool isPasswordCorrect = await _servicesManager.UserManager.CheckPasswordAsync(user, loginModel.Password);
                if (isPasswordCorrect)
                {
                    string token = _servicesManager.UserServices.CreateSignInToken(user);

                    return Ok(new
                    {
                        token
                    });
                }

                return Unauthorized("UserName or Password incorrect...");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

    }
}
