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
        ServicesManager _servicesManager;

        public RegisterController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        public async Task<IActionResult> BasicSignUpAsync(BasicRegisterModel request)
        {

            try
            {
                BaseUser user = _servicesManager.AutoMapper.Map<BasicRegisterModel, BaseUser>(request);
                bool checkPhoneExisted = _servicesManager.UserManager.Users
                    .Any(u => u.PhoneNumber == user.PhoneNumber && u.PhoneNumberConfirmed);

                if (checkPhoneExisted)
                {
                    return BadRequest(new
                    {
                        code= nameof(SignUpFailedStatus.PhoneNumberAlreadyAccount),
                        errors= new string[] { "This phone have already account" }
                    });
                }

                bool checkEmailExist = _servicesManager.UserManager.Users.Any(u => u.Email == user.Email && u.EmailConfirmed);
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

                // Verify email
                user.EmailConfirmed = false;
                user.PhoneNumberConfirmed = false;
                _servicesManager.UserServices.SendEmailToVerifyUser(user);


                // Create Default Actor
                Patient patient = new Patient()
                {
                    BaseUser = user,
                    MedicalRecordFile = new FileMedia()
                    {
                        Category = FileMedia.FileCategory.MedicalRecord
                    }
                };

                _servicesManager.DbContext.Patients.Add(patient);

                var createResult = await _servicesManager.UserManager.CreateAsync(user, request.Password);
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
        EmailAlreadyAccount,
    }
    

}
