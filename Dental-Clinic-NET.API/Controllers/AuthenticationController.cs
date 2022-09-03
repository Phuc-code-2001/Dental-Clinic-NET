using DataLayer.Schemas;
using Dental_Clinic_NET.API.Models.AuthenticationModels;
using Dental_Clinic_NET.API.Serializers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private UserManager<BaseUser> _userManager;
        private IConfiguration _configuration;

        public AuthenticationController(UserManager<BaseUser> userManager, IConfiguration configuration)
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

        [HttpPost]
        public async Task<IActionResult> LoginAsync(BasicLoginModel loginModel)
        {

            try
            {
                BaseUser user = await _userManager.FindByNameAsync(loginModel.UserName);
                if(user != null)
                {
                    bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginModel.Password);
                    if(isPasswordCorrect)
                    {
                        
                        var token = CreateSignInToken(user);

                        return Ok(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expire = token.ValidTo,
                        });
                    }
                }

                return Unauthorized("UserName or Password incorrect...");


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

    
        [NonAction]
        public JwtSecurityToken CreateSignInToken(BaseUser user)
        {
            string role = user.Type.ToString();
            var authClaims = new List<Claim>
            {
                new Claim("Id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Type.ToString()),
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            return token;
        }
    }
}
