using Microsoft.AspNetCore.Http;

namespace Dental_Clinic_NET.API.Models.Technician
{
    public class UploadXRayForm
    {
        public int AppointmentId { get; set; }
        public IFormFile Image { get; set; }
    }
}
