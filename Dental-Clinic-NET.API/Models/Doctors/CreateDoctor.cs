using Microsoft.AspNetCore.Http;
using static DataLayer.Domain.Doctor;

namespace Dental_Clinic_NET.API.Models.Doctors
{
    public class CreateDoctor
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Majors Major { get; set; }
        public IFormFile CertificateFile { get; set; }
        public string Gender { get; set; }


        public string Password { get; set; }
    }
}
