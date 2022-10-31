using DataLayer.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DataLayer.Domain.Room;

namespace Dental_Clinic_NET.API.Models.Room
{
    public class UpdateRoom
    {
        [Required]
        public int Id { get; set; }

        [StringLength(50)]
        public string RoomCode { get; set; }
        public string Description { get; set; }

        public RoomTypes RoomType { get; set; } = RoomTypes.GeneralRoom;

        
    }
}
