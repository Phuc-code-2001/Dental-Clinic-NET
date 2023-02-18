using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Users.UserLock
{
    public class UnlockUserForm
    {
        [Required]
        public string UserId { get; set; }
    }
}
