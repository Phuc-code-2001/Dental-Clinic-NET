using DataLayer.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

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

        // [Required]
        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }

        [Required]
        public DateTime Date { get; set; }
        public bool Status { get; set; } = true;

        [Required]
        public int RoomId { get; set; }

        public List<int> ServiceIdList { get; set; }
    }
}
