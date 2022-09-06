using Dental_Clinic_NET.API.Facebooks.Constracts;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Facebooks.Services
{
    public class FacebookServices : IFacebookServices
    {

        public string TokenValidationUrl = "https://graph.facebook.com/debug_token?input_token={0}&access_token={1}";
        public string UserInfoUrl = "https://graph.facebook.com/me?fields=id,name,picture&access_token={0}";

        public IHttpClientFactory _client;

        public FacebookServices(IHttpClientFactory client)
        {
            _client = client;
        }

        public async Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken)
        {
            
            string formattedUrl = string.Format(TokenValidationUrl, accessToken, accessToken);
            
            HttpResponseMessage result = await _client.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            string responseAsString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FacebookTokenValidationResult>(responseAsString);
        }


        public async Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken)
        {
            string formattedUrl = string.Format(UserInfoUrl, accessToken);

            HttpResponseMessage result = await _client.CreateClient().GetAsync(formattedUrl);
            result.EnsureSuccessStatusCode();

            string responseAsString = await result.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<FacebookUserInfoResult>(responseAsString);
        }

    }
}
