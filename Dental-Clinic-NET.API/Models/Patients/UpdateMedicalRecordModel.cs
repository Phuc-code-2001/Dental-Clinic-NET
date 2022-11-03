using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Patients
{
    public class UpdateMedicalRecordModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        public IFormFile File { get; set; }
    }
}
