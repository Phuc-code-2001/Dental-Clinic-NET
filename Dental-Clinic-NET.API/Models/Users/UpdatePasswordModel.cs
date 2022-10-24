namespace Dental_Clinic_NET.API.Models.Users
{

    public class UpdatePasswordModel
    {
        public string userId { get; set; }
        public string OldPassword { get; set; } 
        public string NewPassword { get; set; }
    }
}
