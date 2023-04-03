using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Doctors
{
    public class DoctorFilter : PageFilter
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public string Major { get; set; }

        public IQueryable<Doctor> GetFilteredQuery(IQueryable<Doctor> source)
        {
            return source.Where(x =>
                (string.IsNullOrWhiteSpace(UserName) || x.BaseUser.UserName.Contains(UserName)) &&
                (string.IsNullOrWhiteSpace(Email) || x.BaseUser.Email.Contains(Email)) &&
                (string.IsNullOrWhiteSpace(PhoneNumber) || x.BaseUser.PhoneNumber.Contains(PhoneNumber)) &&
                (string.IsNullOrWhiteSpace(Major) || x.Major.Contains(Major))
            );
        }
    }
}
