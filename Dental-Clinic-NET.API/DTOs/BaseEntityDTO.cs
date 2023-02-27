using System;

namespace Dental_Clinic_NET.API.DTOs
{
    public abstract class BaseEntityDTO
    {
        public DateTime? TimeCreated { get; set; }
        public DateTime? LastTimeModified { get; set; }
    }
}
