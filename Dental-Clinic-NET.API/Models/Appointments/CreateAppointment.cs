using DataLayer.Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using static DataLayer.Domain.Appointment;

namespace Dental_Clinic_NET.API.Models.Appointments
{
    public class CreateAppointment
    {

        [Required]
        public string PatientId { get; set; }
        [Required]
        public int ServiceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required]
        public TimeManager.SlotManager? Slot { get; set; }

        [Required]
        public string Content { get; set; }

        public IFormFile Document { get; set; }

    }
}
