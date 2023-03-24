using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Technician
{
    public class UploadXRayForm
    {
        [Required]
        public int? AppointmentId { get; set; }
        [Required]
        public IFormFile Image { get; set; }
    }
}
