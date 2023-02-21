namespace Dental_Clinic_NET.API.Models.Users.ForgotPassword
{
    public class SubmitPasswordForm
    {
        
        public string UserName { get; set; }
        public string NewPassword { get; set; }
        public string Secret { get; set; }
    }
}
