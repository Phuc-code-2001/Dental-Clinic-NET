using DataLayer.Domain;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Dental_Clinic_NET.API.DTOs
{
    public class DeviceDTO : BaseEntityDTO
    {
        public int Id { get; set; }
        public int DeviceValue { get; set; }
        public string DeviceName { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }

        public DateTime Date { get; set; }
        public bool Status { get; set; }

        public ServiceInnerDTO[] Services { get; set; }

        public RoomInnerDTO Room { get; set; }

        public class ServiceInnerDTO
        {
            public int Id { get; set; }
            public string ServiceCode { get; set; }
        }

        public class RoomInnerDTO
        {
            public int Id { get; set; }
            public string RoomCode { get; set; }
        }
    }
}
