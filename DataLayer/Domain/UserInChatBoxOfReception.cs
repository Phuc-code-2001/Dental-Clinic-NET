using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class UserInChatBoxOfReception
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(BaseUser))]
        public string UserId { get; set; }
        public BaseUser User { get; set; }

        public bool HasMessageUnRead { get; set; }

        [ForeignKey(nameof(ChatMessage))]
        public int LastMessageId { get; set; }
        [ForeignKey(nameof(LastMessageId))]
        public ChatMessage LastMessage { get; set; }
    }
}
