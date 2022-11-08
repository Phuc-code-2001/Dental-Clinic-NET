using DataLayer.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace Dental_Clinic_NET.API.Services.Users
{
    public class UserServices : IUserServices
    {

        private IConfiguration _configuration;
        private UserManager<BaseUser> _userManager;

        public UserServices(IConfiguration configuration, UserManager<BaseUser> userManager)
        {
            _configuration = configuration;
            _userManager = userManager;
        }

        public string CreateSignInToken(BaseUser user)
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public BaseUser GetLoggedUser(HttpContext context)
        {
            if(!context.User.Identity.IsAuthenticated)
            {
                return null;
            }

            string username = context.User.Identity.Name;
            BaseUser user = _userManager.FindByNameAsync(username).Result;
            return user;
            
        }

        public string GenerateUniqueUserChannel()
        {
            while (true)
            {
                string channel = Guid.NewGuid().ToString();
                if (!_userManager.Users.Any(user => user.PusherChannel == channel))
                {
                    return channel;
                }
            }
        }
    }
}
