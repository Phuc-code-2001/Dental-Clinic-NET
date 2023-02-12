using AutoMapper;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Users;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Serializers;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Services.Users;
using Microsoft.AspNetCore.Authorization;
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

                bool checkEmailExist = _userManager.Users.Any(u => u.Email == user.Email && u.EmailConfirmed);
                if (checkEmailExist)
                {
                    return BadRequest(new
                    {
                        code = nameof(SignUpFailedStatus.EmailAlreadyAccount),
                        errors = new string[] { "This email have already account" }
                    });
                }

                // Generate channel key
                user.PusherChannel = _servicesManager.UserServices.GenerateUniqueUserChannel();

                // Verify email or PhoneNumber
                user.EmailConfirmed = false;
                user.PhoneNumberConfirmed = false;
                _servicesManager.UserServices.SendEmailToVerifyUser(user);


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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequiredConfirmAccountAsync([FromForm] string emailRequired = null)
        {
            try
            {
                BaseUser user = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                if(user.EmailConfirmed)
                {
                    return BadRequest("Your account already verified.");
                }

                if(!string.IsNullOrWhiteSpace(emailRequired))
                {

                    bool duplicate = _userManager.Users.Any(u => u.Email == emailRequired && u.EmailConfirmed);
                    if (duplicate)
                    {
                        return BadRequest($"The email '{emailRequired}' have already account!");
                    }

                    user.Email = emailRequired;
                }
                var checker = await _servicesManager.KickboxServices.VerifyEmailAsync(user.Email);
                if (checker.IsValid)
                {
                    _servicesManager.UserServices.SendEmailToVerifyUser(user);
                    return Ok("We just sent an email to verify your account. Please check your email box include spam email.");
                }
                else
                {
                    return BadRequest("Your required email invalid!");
                }

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EmailVerifyUserAsync([FromQuery] string userId, string code)
        {
            BaseUser user = await _userManager.FindByIdAsync(userId);
            bool succeed = await _servicesManager.UserServices.ConfirmEmailForUser(user, code);

            if(succeed)
            {
                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if(updateResult.Succeeded)
                {
                    return Ok("Tài khoản của bạn đã được xác thực thành công.");
                }
                else
                {
                    throw new Exception("Server error!");
                }

            }
            else
            {
                return BadRequest("Xác thực thất bại. Nguyên nhân có thể do code không đúng hoặc hết hạn.");
            }
        }

    }


    public enum SignUpFailedStatus
    {
        CreatedFailed,
        PhoneNumberAlreadyAccount,
        EmailAlreadyAccount,

        FacebookInValidToken,
        FacebookAlreadySignUp,
        FacebookCreateFailed,
    }
    

}
