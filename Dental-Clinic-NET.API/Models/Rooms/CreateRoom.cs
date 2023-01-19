using DataLayer.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static DataLayer.Domain.Room;

namespace Dental_Clinic_NET.API.Models.Rooms
{
    public class CreateRoom
    {
        [Required]
        [StringLength(50)]
        public string RoomCode { get; set; }
        [Required]
        public string Description { get; set; }

        public RoomTypes RoomType { get; set; } = RoomTypes.GeneralRoom;

    }
}
