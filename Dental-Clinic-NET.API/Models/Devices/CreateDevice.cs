using DataLayer.Domain;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.Models.Devices
{
    public class CreateDevice
    {
        [Required]
        public string DeviceName { get; set; }
        [Required]
        public string Description { get; set; }

        [Required]
        public int? DeviceValue { get; set; }

        // [Required]
        [DataType(DataType.Upload)]
        public IFormFile ImageFile { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        public bool Status { get; set; } = true;

        [Required]
        public int? RoomId { get; set; }

        public HashSet<int> ServiceIdList { get; set; } = new HashSet<int>();
    }
}
