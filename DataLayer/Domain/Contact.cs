using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class Contact : BaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(32)]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        // Non-required
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [StringLength(500)]
        public string Content { get; set; }

        // Action
        public ContactStates State { get; set; } = ContactStates.Pending;
        public DateTime? FinishedTime { get; set; }


    }

    public enum ContactStates
    {
        Pending,
        Done,
        Ignore
    }
}
