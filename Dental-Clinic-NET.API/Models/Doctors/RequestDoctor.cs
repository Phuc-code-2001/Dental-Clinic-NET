using DataLayer.Domain;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using static DataLayer.Domain.Doctor;

namespace Dental_Clinic_NET.API.Models.Doctors
{
    public class RequestDoctor
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public Doctor.Majors Major { get; set; }
        
        public IFormFile CertificateFile { get; set; }

        public bool? Verified { get; set; }
    }
}
