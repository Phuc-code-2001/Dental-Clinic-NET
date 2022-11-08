using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class PatientInChatBoxOfReception
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Patient))]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        public bool HasMessageUnRead { get; set; }
        public DateTime LastMessageCreated { get; set; }

        public int LastMessageId { get; set; }
    }
}
