using DataLayer.Domain;
using Dental_Clinic_NET.API.Services.Users;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using Dental_Clinic_NET.API.Models.Users;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private UserManager<BaseUser> _userManager;
        private UserServices _userServices;

        public LoginController(UserManager<BaseUser> userManager, UserServices userServices)
        {
            _userManager = userManager;
            _userServices = userServices;
        }

        [HttpPost]
        public async Task<IActionResult> LoginBasicAsync(BasicLoginModel loginModel)
        {
            try
            {
                BaseUser user = await _userManager.FindByNameAsync(loginModel.UserName);
                if (user != null)
                {
                    bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginModel.Password);
                    if (isPasswordCorrect)
                    {
                        string token = _userServices.CreateSignInToken(user);

                        return Ok(new
                        {
                            token = token
                        });
                    }
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
