namespace PhoneVerifyServices.Models
{
    public class PhoneRequest
    {
        public string To { get; set; }
        public string From { get; set; }
        public string Body { get; set; }
    }
}
