using DataLayer.Domain;
using static DataLayer.Domain.Doctor;

namespace Dental_Clinic_NET.API.Models.Doctors
{
    public class UpdateDoctor
    {
        public string Id { get; set; }
        public Doctor.Majors Major { get; set; }

    }
}
