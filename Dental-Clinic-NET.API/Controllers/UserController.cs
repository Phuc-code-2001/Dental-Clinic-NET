using DataLayer.Schemas;
using Dental_Clinic_NET.API.Models.AuthenticationModels;
using Dental_Clinic_NET.API.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private UserManager<BaseUser> _userManager;
        private IConfiguration _configuration;

        public UserController(UserManager<BaseUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSuperUserAsync(CreateSuperUserModel inputInfo)
        {

            try
            {

                if (_userManager.Users.Any(user => user.Type == UserType.Administrator))
                {
                    return BadRequest("SuperUser already exist...");
                }

                BaseUser user = inputInfo.ToBaseUser_NotIncludePassword();
                user.Type = UserType.Administrator;
                IdentityResult result = await _userManager.CreateAsync(user, inputInfo.Password);

                if (result.Succeeded)
                {
                    return Ok("Ok...");
                }

                var errors = result.Errors.Select(er => new { er.Code, er.Description });

                return BadRequest(new
                {
                    errorCount = errors.Count(),
                    errors = errors,
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAuthorizeAsync()
        {
            try
            {
                BaseUser authorizeUser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "");
                UserSerializer serializer = new UserSerializer(authorizeUser, authorizeUser);

                return Ok(serializer.Serialize());
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

    }
}
