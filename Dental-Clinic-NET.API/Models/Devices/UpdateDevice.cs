using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Devices
{
    public class UpdateDevice
    {
        [Required]
        public int Id { get; set; }
        public int DeviceValue { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int ServiceDeviceId { get; set; }
        public int RoomId { get; set; }
    }
}
