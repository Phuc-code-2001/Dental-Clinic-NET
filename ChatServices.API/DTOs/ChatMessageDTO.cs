using System;

namespace ChatServices.API.DTOs
{
    public class ChatMessageDTO
    {
        public int Id { get; set; }

        public string Content { get; set; }

        public ChatUserDTO FromUser { get; set; }

        public ChatUserDTO ToUser { get; set; }

        public bool IsRead { get; set; }

        public DateTime? TimeCreated { get; set; }
        public DateTime? LastTimeModified { get; set; }

    }
}
