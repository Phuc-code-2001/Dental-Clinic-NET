using DataLayer.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using Dental_Clinic_NET.API.Services;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VerifyController : ControllerBase
    {

        ServicesManager _servicesManager;

        public VerifyController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RequiredConfirmAccountAsync([FromForm] string emailRequired = null)
        {
            try
            {
                BaseUser user = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                if (user.EmailConfirmed)
                {
                    return BadRequest("Your account already verified.");
                }

                if (!string.IsNullOrWhiteSpace(emailRequired))
                {

                    bool duplicate = _servicesManager.UserManager.Users.Any(u => u.Email == emailRequired && u.EmailConfirmed);
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> EmailVerifyUserAsync([FromQuery] string userId, string code)
        {
            BaseUser user = await _servicesManager.UserManager.FindByIdAsync(userId);
            bool succeed = await _servicesManager.UserServices.ConfirmEmailForUser(user, code);

            if (succeed)
            {
                user.EmailConfirmed = true;
                var updateResult = await _servicesManager.UserManager.UpdateAsync(user);

                if (updateResult.Succeeded)
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
}
