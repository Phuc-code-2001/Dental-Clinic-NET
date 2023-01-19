using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class EmailConfirmation : BaseEntity
    {
        [Key]
        [ForeignKey(nameof(BaseUser))]
        public string UserId { get; set; }

        public string LastRequiredCode { get; set; }
        public DateTime ValidTo { get; set; }

    }
}
