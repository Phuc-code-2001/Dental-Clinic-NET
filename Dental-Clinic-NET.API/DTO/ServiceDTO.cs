using DataLayer.Domain;
using System.Collections.Generic;

namespace Dental_Clinic_NET.API.DTO
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string ServiceCode { get; set; }
        public string ServiceName { get; set; }
        public string ImageURL { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }

        public EnumTypeDTO[] Devices { get; set; } 

    }
}
