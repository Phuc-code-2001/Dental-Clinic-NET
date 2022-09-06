using DataLayer.Schemas;
using System.IdentityModel.Tokens.Jwt;

namespace Dental_Clinic_NET.API.Services.UserServices
{
    public interface IUserServices
    {
        public string CreateSignInToken(BaseUser user);
        
    }
}
