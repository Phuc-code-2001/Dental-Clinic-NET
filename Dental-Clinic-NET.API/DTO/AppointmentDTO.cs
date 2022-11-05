using DataLayer.Domain;

namespace Dental_Clinic_NET.API.DTO
{
    public class AppointmentDTO : BaseEntityDTO
    {
        public int Id { get; set; }

        public DoctorDTO Doctor { get; set; }

        public PatientDTO Patient { get; set; }

        public RoomDTO Room { get; set; }

        public ServiceDTO Service { get; set; }

        public string Content { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public string State { get; set; }
    }
}
