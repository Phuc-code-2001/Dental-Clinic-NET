using DataLayer.Domain;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users
{
    public class CreateSuperUserModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string FullName { get; set; }
        
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public string ImageURL { get; set; }

    }
}
