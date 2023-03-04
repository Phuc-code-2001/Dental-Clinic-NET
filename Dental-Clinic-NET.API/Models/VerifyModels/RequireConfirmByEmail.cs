using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.VerifyModels
{
    public class RequireConfirmByEmail
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}
