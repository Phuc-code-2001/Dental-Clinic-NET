using DataLayer.Schemas;
using Dental_Clinic_NET.API.Facebooks.Models;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Models.Users;
using Dental_Clinic_NET.API.Serializers;
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

        private UserServices _userServices;
        private FacebookServices _facebookServices;

        public RegisterController(UserManager<BaseUser> userManager, FacebookServices facebookServices, UserServices userServices)
        {
            _userManager = userManager;
            _facebookServices = facebookServices;
            _userServices = userServices;
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
                    string token = _userServices.CreateSignInToken(user);
                    UserSerializer serializer = new UserSerializer(user, user);
                    return Ok(new
                    {
                        id = user.Id,
                        token = token,
                        user = serializer.Serialize(),
                    });
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


        [HttpPost]
        public async Task<IActionResult> BasicSignUpAsync(BasicRegisterModel request)
        {

            try
            {
                BaseUser user = request.ToBaseUser_NotIncludePassword();
                if(await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == user.PhoneNumber) != null)
                {
                    return BadRequest(new
                    {
                        code= SignUpFailedStatus.PhoneNumberAlreadyAccount,
                        errors= new string[] { "This phone have already account" }
                    });
                }

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if(createResult.Succeeded)
                {
                    string token = _userServices.CreateSignInToken(user);
                    UserSerializer serializer = new UserSerializer(user, user);
                    return Ok(new
                    {
                        id=user.Id,
                        token=token,
                        user=serializer.Serialize(),
                    });
                }

                var errors = createResult.Errors.Select(e => new { e.Code, e.Description });

                return BadRequest(new
                {
                    code=SignUpFailedStatus.CreatedFailed,
                    errors=errors
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
        CreatedFailed,
        PhoneNumberAlreadyAccount,

        FacebookInValidToken,
        FacebookAlreadySignUp,
        FacebookCreateFailed,
    }
}
