using DataLayer.Schemas;
using Dental_Clinic_NET.API.Facebooks.Models;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Services.UserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private UserManager<BaseUser> _userManager;
        private IConfiguration _configuration;

        private IHttpClientFactory _client;

        private UserServices _userServices;
        private FacebookServices _facebookServices;

        public RegisterController(UserManager<BaseUser> userManager, IConfiguration configuration, IHttpClientFactory client)
        {
            _userManager = userManager;
            _configuration = configuration;
            _client = client;
            _userServices = new UserServices(configuration);
            _facebookServices = new FacebookServices(client);
        }

        [HttpPost]
        public async Task<IActionResult> SignUpWithFacebookAsync(FacebookRegisterModel request)
        {

            try
            {
                string fbToken = request.AccessToken;
                var result = await _facebookServices.ValidateAccessTokenAsync(fbToken);

                if (!result.Data.IsValid)
                {
                    return BadRequest(new
                    {
                        code=SignUpFailedStatus.FacebookInValidToken,
                        errors=new string[] {"Invalid Token"}
                    });
                }

                var fbUserInfo = await _facebookServices.GetUserInfoAsync(fbToken);

                BaseUser user = await _userManager.Users.Where(u => u.FbConnectedId == fbUserInfo.Id).FirstOrDefaultAsync();

                if (user != null)
                {
                    return BadRequest(new
                    {
                        code = SignUpFailedStatus.FacebookAlreadySignUp,
                        errors = new string[] { "Your facebook have already account." }
                    });
                }

                user = new BaseUser()
                {
                    UserName = request.UserName,
                    FullName = fbUserInfo.Name,
                    FbConnectedId = fbUserInfo.Id,
                    ImageURL = fbUserInfo.Picture.Data.Url.ToString(),
                };


                // Verify Email and PhoneNumber later

                var createUserResult = await _userManager.CreateAsync(user);

                if(createUserResult.Succeeded)
                {
                    return Ok("Sign Up Succeeded");
                }

                var errors = createUserResult.Errors.Select(e => new { e.Code, e.Description });

                return BadRequest(new
                {
                    code = SignUpFailedStatus.FacebookCreateFailed,
                    errors = errors
                });

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }


    public enum SignUpFailedStatus
    {
        FacebookInValidToken,
        FacebookAlreadySignUp,
        FacebookCreateFailed,
    }
}
