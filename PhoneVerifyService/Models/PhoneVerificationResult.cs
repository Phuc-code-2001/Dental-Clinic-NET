using KickBox.Core.Models;

namespace MailServices.Models
{
    public class PhoneVerificationResult
    {
        public bool IsSucceed { get; set; } = false;
        public bool IsValid { get; set; } = false;
        public object Details { get; set; }
    }
}
