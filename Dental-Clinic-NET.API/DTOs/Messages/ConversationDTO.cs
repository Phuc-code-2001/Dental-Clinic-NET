using System;

namespace Dental_Clinic_NET.API.DTOs.Messages
{
    public class ConversationDTO
    {
        public int Id { get; set; }

        public ChatUserDTO User { get; set; }
        public bool HasMessageUnRead { get; set; }
        public int LastMessageId { get; set; }
        public DateTime LastMessageCreated { get; set; }

        public string PreviewContent { get; set; }
        public string TimeFormatted { get; set; }

    }
}
