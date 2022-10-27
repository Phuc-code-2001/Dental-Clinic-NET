using DataLayer.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dental_Clinic_NET.API.Models.Room
{
    public class CreateRoom
    {
        [Required]
        [StringLength(50)]
        public string RoomCode { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
