using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class Appointment
    {

        [Key]
        public int Id { get; set; }

        [ForeignKey("Doctor")]
        public string DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        [ForeignKey("Patient")]
        public string PatientId { get; set; }
        public Patient Patient { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public TimeManager.Slot Slot { get; set; }

        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        public States State { get; set; } = States.NotYet;

        public ICollection<Service> Services { get; set; }

        public enum States
        {
            NotYet,
            Cancel,
            Complete,
        }
    }
    
}
