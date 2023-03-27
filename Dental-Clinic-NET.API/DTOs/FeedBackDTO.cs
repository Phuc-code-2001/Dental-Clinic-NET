using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.DTOs
{
    public class FeedBackDTO : BaseEntityDTO
    {
        public int Id { get; set; }

        public UserDTOLite User { get; set; }

        public float RatingPoint { get; set; }

        public string Content { get; set; }
    }
}
