using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class UserFilter : PageFilter
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public bool? EmailVerified { get; set; }
        public bool? IsLock { get; set; }

        public IQueryable<BaseUser> GetFilteredQuery(IQueryable<BaseUser> source)
        {
            return source.Where(x =>
                (string.IsNullOrWhiteSpace(UserName) || x.UserName.Contains(UserName)) &&
                (string.IsNullOrWhiteSpace(Email) || x.Email.Contains(Email)) &&
                (string.IsNullOrWhiteSpace(PhoneNumber) || x.PhoneNumber.Contains(PhoneNumber)) &&
                (EmailVerified == null || x.EmailConfirmed == EmailVerified.Value)
            );
        } 

    }
}
