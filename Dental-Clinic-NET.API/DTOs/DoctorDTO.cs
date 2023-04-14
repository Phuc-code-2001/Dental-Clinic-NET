using DataLayer.Domain;
using static DataLayer.Domain.Doctor;

namespace Dental_Clinic_NET.API.DTOs
{
    public class DoctorDTO : BaseEntityDTO
    {
        public string Id { get; set; }
        public UserDTO BaseUser { get; set; }
        public string Major { get; set; }
        public MediaFileDTO Certificate { get; set; }
        public bool Verified { get; set; }
    }

    public class DoctorDTOLite : BaseEntityDTO
    {
        public string Id { get; set; }
        public UserDTOLite BaseUser { get; set; }
    }
}
