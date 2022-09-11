using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class BasicLoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
