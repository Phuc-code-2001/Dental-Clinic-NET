using MailServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MailServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailServicesController : ControllerBase
    {
        KickboxServices _kickBoxServices;
        EmailSender _emailSender;

        public EmailServicesController(KickboxServices kickBoxServices, EmailSender emailSender)
        {
            _kickBoxServices = kickBoxServices;
            _emailSender = emailSender;
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail([EmailAddress] string email)
        {
            try
            {
                var verifyResult = await _kickBoxServices.VerifyEmailAsync(email);
                return Ok(verifyResult);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string toEmail, string subject, string htmlMessage)
        {
            try
            {
                var verifyResult = await _kickBoxServices.VerifyEmailAsync(toEmail);
                if(verifyResult.IsValid)
                {
                    await _emailSender.SendEmailAsync(toEmail, subject, htmlMessage);
                    return Ok("Sent succeed.");
                }
                else
                {
                    return BadRequest("Can not verify email!");
                }
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            
        }

    }
}
