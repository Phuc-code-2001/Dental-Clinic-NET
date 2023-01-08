using MailServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace MailServices.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmailServicesController : ControllerBase
    {
        KickboxServices _kickBoxServices;

        public EmailServicesController(KickboxServices kickBoxServices)
        {
            _kickBoxServices = kickBoxServices;
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail([EmailAddress] string email)
        {
            
            var verifyResult = await _kickBoxServices.VerifyEmailAsync(email);
            return Ok(verifyResult);
        }

    }
}
