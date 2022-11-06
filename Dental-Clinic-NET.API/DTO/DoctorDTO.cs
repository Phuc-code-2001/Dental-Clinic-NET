using DataLayer.Domain;
using static DataLayer.Domain.Doctor;

namespace Dental_Clinic_NET.API.DTO
{
    public class DoctorDTO : BaseEntityDTO
    {
        public string Id { get; set; }
        public UserDTO BaseUser { get; set; }
        public EnumTypeDTO Major { get; set; }
        public MediaFileDTO Certificate { get; set; }
        public bool Verified { get; set; }
    }
}
