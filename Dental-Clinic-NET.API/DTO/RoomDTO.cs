using DataLayer.Domain;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.DTO
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomCode { get; set; }
        public string Description { get; set; }
        public List<string> DeviceNames { get; set; }
    }
}
