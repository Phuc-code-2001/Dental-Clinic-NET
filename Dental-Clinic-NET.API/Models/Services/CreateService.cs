using DataLayer.Domain;
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
        public int Price { get; set; }
    }
}
