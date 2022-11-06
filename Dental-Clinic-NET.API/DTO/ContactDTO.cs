using DataLayer.Domain;
using System;

namespace Dental_Clinic_NET.API.DTO
{
    public class ContactDTO : BaseEntityDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        public string Email { get; set; }

        public string Content { get; set; }

        public string State { get; set; }
        public DateTime? FinishedTime { get; set; }

    }
}
