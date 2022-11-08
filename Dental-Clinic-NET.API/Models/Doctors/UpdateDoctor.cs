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

        public Nullable<Majors> Major { get; set; }
        public IFormFile CertificateFile { get; set; }
        public Nullable<bool> Verified { get; set; }
    }
}
