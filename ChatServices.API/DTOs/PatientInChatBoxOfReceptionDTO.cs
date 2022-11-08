using System;

namespace ChatServices.API.DTOs
{
    public class PatientInChatBoxOfReceptionDTO
    {
        public int Id { get; set; }

        public ChatUserDTO User { get; set; }
        public bool HasMessageUnRead { get; set; }
        public DateTime LastMessageCreated { get; set; }

        public int LastMessageId { get; set; }

    }
}
