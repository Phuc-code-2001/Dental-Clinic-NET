using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Users.ForgotPassword;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        ServicesManager _servicesManager;

        public ForgotPasswordController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordByEmailAsync(ForgotPasswordForm form, [FromQuery] string prefixUrl)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByNameAsync(form.UserName);
                if(requiredUser == null)
                {
                    return NotFound("User not found!");
                }

                if (!requiredUser.EmailConfirmed)
                {
                    return BadRequest("No have any email verified!");
                }

                string verifiedEmail = requiredUser.NormalizedEmail;
                string secret = await _servicesManager.UserManager.GeneratePasswordResetTokenAsync(requiredUser);

                Task sendMailTask = _servicesManager.EmailSender.SendEmailAsync(
                        email: verifiedEmail,
                        subject: "Reset password instruction\n",
                        htmlMessage: $"We just receive your reset password request for account '{requiredUser.UserName}'.\n" +
                        $"Click this link to submit new password: {prefixUrl}?secret={secret}\n" +
                        $"Please keep this link and do not share with anyone. This link is valid in 3 minutes."
                    );

                return Ok("We just sent a email with the instruction to reset password. Please check your mailbox.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SubmitPasswordAsync(SubmitPasswordForm form)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByNameAsync(form.UserName);
                if (requiredUser == null)
                {
                    return NotFound("User not found!");
                }

                var resetPasswordResult = await _servicesManager.UserManager
                    .ResetPasswordAsync(requiredUser, form.Secret, form.NewPassword);
                
                if(resetPasswordResult.Succeeded)
                {
                    return Ok("Your password was reset success.");
                }

                return BadRequest("Your request data invalid!");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

    }
}
