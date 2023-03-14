using System.Collections.Generic;
using System;

namespace Dental_Clinic_NET.API.Models.Posts
{
    public class UpdatePost
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.Now;

        public List<int> ServicesId { get; set; } = new List<int>();
    }
}
