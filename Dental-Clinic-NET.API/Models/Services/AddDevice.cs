using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Models.Services
{
    public class AddDevice
    {
        public int Id { get; set; }
        public IList<int> ListDeviceId { get; set; }
    }
}
