using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.Devices
{
    public class UpdateDevice
    {
        [Required]
        public int Id { get; set; }

        public int? DeviceValue { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }

        public DateTime? Date { get; set; }
        public bool? Status { get; set; } = true;

        public int? RoomId { get; set; }

        public List<int> ServiceIdList { get; set; } = new List<int>();
    }
}
