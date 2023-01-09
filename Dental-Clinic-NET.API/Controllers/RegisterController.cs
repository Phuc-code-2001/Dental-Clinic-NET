using AutoMapper;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Facebooks.Models;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Models.Users;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Serializers;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Services.Users;
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
        AppDbContext _context;
        private ServicesManager _servicesManager;
        private UserManager<BaseUser> _userManager;

        public RegisterController(AppDbContext context, ServicesManager servicesManager, UserManager<BaseUser> userManager)
        {
            _context = context;
            _servicesManager = servicesManager;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> SignUpWithFacebookAsync(FacebookRegisterModel request)
        {

            try
            {
                string fbToken = request.AccessToken;
                var result = await _servicesManager.FacebookServices.ValidateAccessTokenAsync(fbToken);

                if (!result.Data.IsValid)
                {
                    return BadRequest(new
                    {
                        code=nameof(SignUpFailedStatus.FacebookInValidToken),
                        errors=new string[] {"Invalid Token"}
                    });
                }

                var fbUserInfo = await _servicesManager.FacebookServices.GetUserInfoAsync(fbToken);

                BaseUser user = await _userManager.Users.Where(u => u.FbConnectedId == fbUserInfo.Id).FirstOrDefaultAsync();

                if (user != null)
                {
                    return BadRequest(new
                    {
                        code = nameof(SignUpFailedStatus.FacebookAlreadySignUp),
                        errors = new string[] { "Your facebook have already account." }
                    });
                }

                user = new BaseUser()
                {
                    UserName = request.UserName,
                    FullName = fbUserInfo.Name,
                    FbConnectedId = fbUserInfo.Id,
                    ImageURL = fbUserInfo.Picture.Data.Url.ToString(),
                    // Generate channel key
                    PusherChannel = _servicesManager.UserServices.GenerateUniqueUserChannel(),
                };

                // Verify Email and PhoneNumber later



                // Create Default Actor
                Patient patient = new Patient()
                {
                    BaseUser = user,
                    MedicalRecordFile = new MediaFile()
                    {
                        Category = MediaFile.FileCategory.MedicalRecord
                    }
                };

                var createUserResult = await _userManager.CreateAsync(user);

                if(createUserResult.Succeeded)
                {
                
                    string token = _servicesManager.UserServices.CreateSignInToken(user);
                    UserSerializer serializer = new UserSerializer(new PermissionOnBaseUser(user, user));
                    return Ok(new
                    {
                        id = user.Id,
                        token,
                        user = serializer.Serialize(user =>
                        {
                            return _servicesManager.AutoMapper.Map<UserDTO>(user);
                        }),
                    });
                }

                var errors = createUserResult.Errors.Select(e => new { e.Code, e.Description });

                return BadRequest(new
                {
                    code = nameof(SignUpFailedStatus.FacebookCreateFailed),
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
                BaseUser user = _servicesManager.AutoMapper.Map<BasicRegisterModel, BaseUser>(request);
                bool checkPhoneExisted = _userManager.Users.Any(u => u.PhoneNumber == user.PhoneNumber);

                if (checkPhoneExisted)
                {
                    return BadRequest(new
                    {
                        code= nameof(SignUpFailedStatus.PhoneNumberAlreadyAccount),
                        errors= new string[] { "This phone have already account" }
                    });
                }

                // Generate channel key
                user.PusherChannel = _servicesManager.UserServices.GenerateUniqueUserChannel();

                // Verify email or PhoneNumber
                user.EmailConfirmed = false;
                user.PhoneNumberConfirmed = false;

                // Create Default Actor
                Patient patient = new Patient()
                {
                    BaseUser = user,
                    MedicalRecordFile = new MediaFile()
                    {
                        Category = MediaFile.FileCategory.MedicalRecord
                    }
                };

                _context.Patients.Add(patient);

                var createResult = await _userManager.CreateAsync(user, request.Password);
                if(createResult.Succeeded)
                {

                    string token = _servicesManager.UserServices.CreateSignInToken(user);
                    UserSerializer serializer = new UserSerializer(new PermissionOnBaseUser(user, user));
                    return Ok(new
                    {
                        id=user.Id,
                        token=token,
                        user=serializer.Serialize(user =>
                        {
                            return _servicesManager.AutoMapper.Map<UserDTO>(user);
                        }),
                    });
                }

                var errors = createResult.Errors.Select(e => new { e.Code, e.Description });

                return BadRequest(new
                {
                    code=nameof(SignUpFailedStatus.CreatedFailed),
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
