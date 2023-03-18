using System.ComponentModel.DataAnnotations;

namespace Dental_Clinic_NET.API.Models.FeedBacks
{
    public class FeedBackCreation
    {
        [Required]
        public int? AppointmentId { get; set; }
        [Required]
        [Range(0, 5)]
        public float? RatingPoint { get; set; }
        public string Content { get; set; }

    }
}
