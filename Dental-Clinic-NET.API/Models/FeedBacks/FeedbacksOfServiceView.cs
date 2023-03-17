using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Models.FeedBacks
{
    public class FeedbacksOfServiceView
    {
        public int Total { get; set; }
        public float AverageRatingPoint { get; set; }
        public Dictionary<float, float> Percentages { get; set; }
        public dynamic Items { get; set; }

    }
}
