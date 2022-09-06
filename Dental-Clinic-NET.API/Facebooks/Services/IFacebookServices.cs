using Dental_Clinic_NET.API.Facebooks.Constracts;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Facebooks.Services
{
    public interface IFacebookServices
    {
        public Task<FacebookTokenValidationResult> ValidateAccessTokenAsync(string accessToken);
        public Task<FacebookUserInfoResult> GetUserInfoAsync(string accessToken);
    }
}
