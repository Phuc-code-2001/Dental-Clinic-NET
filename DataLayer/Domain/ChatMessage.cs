using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class ChatMessage : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public string Content { get; set; }

        [ForeignKey(nameof(BaseUser))]
        public string FromId { get; set; }
        [ForeignKey(nameof(FromId))]
        public BaseUser FromUser { get; set; }

        [ForeignKey(nameof(BaseUser))]
        public string ToId { get; set; }
        [ForeignKey(nameof(ToId))]
        public BaseUser ToUser { get; set; }

        public bool IsRead { get; set; }

    }
}
