using System.Collections.Generic;

namespace Dental_Clinic_NET.API.Models.Devices
{
    public class AddService
    {
        public int Id { get; set; }
        public IList<int> ListServiceId { get; set; }
    }
}
