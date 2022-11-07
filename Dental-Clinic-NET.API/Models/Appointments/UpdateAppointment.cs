using DataLayer.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Appointments
{
    public class UpdateAppointment
    {
        [Required]
        public int Id { get; set; }
        
        public int? ServiceId { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        public TimeManager.Slot? Slot { get; set; }

        public string Content { get; set; }

    }
}
