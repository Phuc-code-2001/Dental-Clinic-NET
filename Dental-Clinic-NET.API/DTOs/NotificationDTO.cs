namespace Dental_Clinic_NET.API.DTOs
{
    public class NotificationDTO
    {
        public UserDTOLite Receiver { get; set; }
        public string Content { get; set; }
        public bool Clicked { get; set; }
        public string Url { get; set; }

        public string Category { get; set; }

        public bool Hidden { get; set; }
        
        public string CreatedFormated { get; set; }
    }
}
