using DataLayer.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Devices
{
    public class CreateDevice
    {
        [Required]
        public int DeviceValue { get; set; }
        [Required]
        public string DeviceName { get; set; }
        [Required]
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool Status { get; set; }
        [Required]
        public int RoomId { get; set; }
    }
}
