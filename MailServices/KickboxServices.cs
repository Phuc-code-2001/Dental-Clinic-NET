using KickBox.Core;
using KickBox.Core.Models;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using MailServices.Models;

namespace MailServices
{
    public class KickboxServices
    {
        KickBoxApi _kickboxClient;

        public KickboxServices(IConfiguration configuration)
        {
            _kickboxClient = new KickBoxApi(
                apiKey: configuration["Kickbox:SandBoxApiKey"],
                apiUrl: "https://api.kickbox.com/v2"
            );
        }

        public async Task<EmailVerificationResult> VerifyEmailAsync(string email)
        {
            EmailVerificationResult result = new EmailVerificationResult();

            try
            {
                KickBoxResponse response = await _kickboxClient.VerifyEmail(new System.Net.Mail.MailAddress(email));

                if (response.Success)
                {
                    result.IsSucceed = true;
                    result.IsValid = response.Result == Result.Deliverable;
                    result.Details = response;
                }
                else
                {
                    result.IsSucceed = false;
                    result.Details = response;
                }
            }
            catch (Exception ex)
            {
                result.IsSucceed = false;
                result.Details = ex.Message;
            }


            return result;
        }

    }
}
