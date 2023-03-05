namespace Dental_Clinic_NET.API.DTOs.Messages
{
    public class ChatUserDTO
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string ImageURL { get; set; }

        public string UserRole { get; set; }
    }
}
