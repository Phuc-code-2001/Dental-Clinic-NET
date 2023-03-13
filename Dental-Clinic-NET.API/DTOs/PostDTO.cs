using DataLayer.Domain;
using System.Collections.Generic;
using System;

namespace Dental_Clinic_NET.API.DTOs
{
    public class PostDTO : BaseEntityDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Creator { get; set; }
        public DateTime PublishDate { get; set; }

        public List<ServiceDTOLite> Services { get; set; }
    }
}
