using DataLayer.Domain;
using System;

namespace Dental_Clinic_NET.API.DTOs
{
    public class AppointmentDTO : BaseEntityDTO
    {
        public int Id { get; set; }

        public DoctorDTO Doctor { get; set; }

        public PatientDTO Patient { get; set; }

        public RoomDTO Room { get; set; }

        public ServiceDTO Service { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string State { get; set; }

        public DocumentDTO[] Documents { get; set; }
    }

    public class AppointmentDTOLite : BaseEntityDTO
    {
        public int Id { get; set; }

        public DoctorDTOLite Doctor { get; set; }

        public PatientDTOLite Patient { get; set; }

        public RoomDTOLite Room { get; set; }

        public ServiceDTOLite Service { get; set; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string State { get; set; }
    }
}
