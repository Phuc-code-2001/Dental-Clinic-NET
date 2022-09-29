using DataLayer.Domain;
using System.IdentityModel.Tokens.Jwt;

namespace Dental_Clinic_NET.API.Services.Users
{
    public interface IUserServices
    {
        public string CreateSignInToken(BaseUser user);
    }
}
