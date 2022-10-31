using DataLayer.Domain;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.DTO
{
    public class RoomDTO
    {
        public int Id { get; set; }
        public string RoomCode { get; set; }
        public string Description { get; set; }
        public List<DeviceInnerDTO> Devices { get; set; }
        public string RoomType { get; set; }

        public class DeviceInnerDTO
        {
            public int Id { get; set; }
            public string DeviceName { get; set; }
        }
    }
}
