using Dental_Clinic_NET.API.DTOs;

namespace Dental_Clinic_NET.API.Models.FeedBacks
{
    public class ServiceRanking
    {
        public object ServiceInfo { get; set; }
        public float AveragePoint { get; set; }
        public int FeedBackCount { get; set; }
    }
}
