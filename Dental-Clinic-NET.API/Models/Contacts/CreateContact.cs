using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Contacts
{
    public class CreateContact
    {
        
        [Required]
        [StringLength(32)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        // Non-required
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }
    }
}
