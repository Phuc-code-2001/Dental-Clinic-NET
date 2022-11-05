using System;

namespace Dental_Clinic_NET.API.DTO
{
    public abstract class BaseEntityDTO
    {
        public DateTime? TimeCreated { get; set; } = DateTime.Now;
        public DateTime? LastTimeModified { get; set; }
    }
}
