using DataLayer.Schemas;
using Dental_Clinic_NET.API.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Dental_Clinic_NET.API.Facebooks.Models;
using Dental_Clinic_NET.API.Facebooks.Services;
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
        private FacebookServices _facebookServices;
        private UserServices _userServices;

        public LoginController(UserManager<BaseUser> userManager, FacebookServices facebookServices, UserServices userServices)
        {
            _userManager = userManager;
            _userServices = userServices;
            _facebookServices = facebookServices;
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

        [HttpPost]
        public async Task<IActionResult> LoginWithFacebookAsync(FacebookLoginModel request)
        {
            try
            {
                string fbToken = request.AccessToken;
                var result = await _facebookServices.ValidateAccessTokenAsync(fbToken);

                if(!result.Data.IsValid)
                {
                    return BadRequest("InvalidToken");
                }

                var fbUserInfo = await _facebookServices.GetUserInfoAsync(fbToken);

                var user = await _userManager.Users.Where(u => u.FbConnectedId == fbUserInfo.Id).FirstOrDefaultAsync();

                if(user == null)
                {
                    return BadRequest("RegisterRequired");
                }
                
                string token = _userServices.CreateSignInToken(user);

                return Ok(new
                {
                    token = token
                });
               
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
