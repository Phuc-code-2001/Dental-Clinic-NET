using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Facebooks.Models
{
    public class FacebookLoginModel
    {
        [Required]
        public string AccessToken { get; set; }
    }
}
