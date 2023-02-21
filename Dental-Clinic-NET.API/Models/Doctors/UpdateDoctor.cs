using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using static DataLayer.Domain.Doctor;

namespace Dental_Clinic_NET.API.Models.Doctors
{
    public class UpdateDoctor
    {
        [Required]
        public string Id { get; set; }

        public string Major { get; set; }
        public IFormFile CertificateFile { get; set; }
        public bool? Verified { get; set; }
    }
}
