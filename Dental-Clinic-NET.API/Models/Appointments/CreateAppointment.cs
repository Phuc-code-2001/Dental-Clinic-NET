using DataLayer.Extensions;
using Dental_Clinic_NET.API.Models.Schedules;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Appointments
{
    public class CreateAppointment : TimeIdentifier
    {

        [Required]
        public string PatientId { get; set; }
        [Required]
        public int ServiceId { get; set; }
        
        public string DoctorId { get; set; }

        [Required]
        public string Content { get; set; }
        public IFormFile Document { get; set; }


    }
}
