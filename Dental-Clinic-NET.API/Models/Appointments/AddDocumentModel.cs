using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Appointments
{
    public class AddDocumentModel
    {
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        public IFormFile DocumentFile { get; set; }
 
        public string Title { get; set; }

    }
}
