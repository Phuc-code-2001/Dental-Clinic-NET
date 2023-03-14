using DataLayer.Domain;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental_Clinic_NET.API.Models.Services
{
    public class CreateService
    {
        [Required]
        public string ServiceCode { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ServiceName { get; set; }

        public IFormFile ImageFile { get; set; }

        [Required]
        public int? Price { get; set; }

        public List<int> DeviceIdList { get; set; } = new List<int>();

        public bool IsPublic { get; set; } = true;

    }
}
