using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Models.Services
{
    public class UpdateService
    {
        [Required]
        public int Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public IFormFile ImageFile { get; set; }
        public string Description { get; set; }

        public int? Price { get; set; }

        public List<int> DeviceIdList { get; set; } = new List<int>();
    }
}
