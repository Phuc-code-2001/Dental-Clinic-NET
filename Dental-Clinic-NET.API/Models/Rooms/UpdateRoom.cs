using DataLayer.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental_Clinic_NET.API.Models.Room
{
    public class UpdateRoom
    {
        [Required]
        public int Id { get; set; }
        [StringLength(50)]
        public string RoomCode { get; set; }
        public string Description { get; set; }
    }
}
