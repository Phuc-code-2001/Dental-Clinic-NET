using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class FeedBack : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public BaseUser User { get; set; }
        public int ServiceId { get; set; }
        public int AppointmentId { get; set; }

        public float RatingPoint { get; set; }
        
        public string Content { get; set; }

    }
}
