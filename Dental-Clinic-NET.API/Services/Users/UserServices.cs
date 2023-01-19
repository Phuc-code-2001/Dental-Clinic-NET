using DataLayer.DataContexts;
using DataLayer.Domain;
using MailServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Services.Users
{
    public class UserServices : IUserServices
    {

        private AppDbContext _dbContext;
        private IConfiguration _configuration;
        private UserManager<BaseUser> _userManager;
        private EmailSender _emailSender;

        public UserServices(IConfiguration configuration, UserManager<BaseUser> userManager, EmailSender emailSender, AppDbContext dbContext)
        {
            _configuration = configuration;
            _userManager = userManager;
            _emailSender = emailSender;
            _dbContext = dbContext;
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


        private static Dictionary<string, EmailConfirmation> EmailConfirmarionCache = new Dictionary<string, EmailConfirmation>();

        public async void SendEmailToVerifyUser(BaseUser user, Action<EmailConfirmation> callback = null)
        {

            try
            {
                string code = (new Random()).Next(0, 999999).ToString("000000");

                EmailConfirmation confirmation = GetOrCreateEmailConfirmation(user);
                confirmation.LastRequiredCode = code;
                confirmation.ValidTo = DateTime.Now + TimeSpan.FromSeconds(Double.Parse(_configuration["EmailVerification:CodeExpireTime"]));

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Xác nhận email của bạn",
                    $"Đây là mã xác thực email của bạn, mã có hiệu lực trong vòng 3 phút:\n" +
                    $"Code: {code} " +
                    $"Vui lòng không cung cấp mã này cho bất kì ai. " +
                    $"Việc xác thực tài khoản giúp bạn có thể thực hiện các tác vụ sau này như quên mật khẩu, ...");

                EmailConfirmarionCache.Add(user.Email, confirmation);
                if(callback != null) callback(confirmation);
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendEmailToVerifyUser Exception: ", ex.Message);
            }
            finally
            {

            }

        }

        public EmailConfirmation GetOrCreateEmailConfirmation(BaseUser user)
        {
            EmailConfirmation confirmation;
            bool exist = UserServices.EmailConfirmarionCache.TryGetValue(user.Email, out confirmation);

            if (!exist)
            {
                confirmation = new EmailConfirmation()
                {
                    UserId = user.Id
                };
            }

            return confirmation;

        }

        public async Task<bool> ConfirmEmailForUser(BaseUser user, string code)
        {
            EmailConfirmation confirmation;
            bool exist = UserServices.EmailConfirmarionCache.TryGetValue(user.Email, out confirmation);

            if (exist && confirmation.LastRequiredCode.Equals(code) && confirmation.ValidTo >= DateTime.Now)
            {

                _dbContext.EmailConfirmations.Add(confirmation);
                _dbContext.SaveChanges();
                UserServices.EmailConfirmarionCache.Remove(user.Email);
                return await Task.FromResult(true);
            }

            return await Task.FromResult(false);
        }

    }
}
