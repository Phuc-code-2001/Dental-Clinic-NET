using Microsoft.Extensions.Configuration;
using PhoneVerifyServices.Models;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Rest.Api.V2010.Account.Message;
using Twilio.TwiML.Voice;

namespace PhoneVerifyServices
{
    public class PhoneVerifyService
    {
        private string _accountSID;
        private string _authToken;

        public PhoneVerifyService(IConfiguration configuration)
        {
            _accountSID = configuration["Twilio:AccountSid"];
            _authToken = configuration["Twilio:AuthToken"];
            TwilioClient.Init(configuration["Twilio:AccountSid"], configuration["Twilio:AuthToken"]);
        }
        public async Task<bool> PhoneVerification(PhoneRequest phoneRequest)
        {
            // Send the SMS message using Twilio
            var message = await MessageResource.CreateAsync(
                to: phoneRequest.To,
                from: phoneRequest.From,
                body: phoneRequest.Body
            );
            return false;
        }
    }
}
