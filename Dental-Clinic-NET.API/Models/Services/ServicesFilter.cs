using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Services
{
    public class ServicesFilter : PageFilter
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public int? MinPrice { get; set; } = int.MinValue;
        public int? MaxPrice { get; set; } = int.MaxValue;

        public bool Union { get; set; } = false;

        public IQueryable<Service> GetFilteredQuery(IQueryable<Service> src)
        {
            return src.Where(x =>
                (string.IsNullOrWhiteSpace(Code) || x.ServiceCode.Contains(Code)) &&
                (string.IsNullOrWhiteSpace(Name) || x.ServiceName.Contains(Name)) &&
                (MinPrice == null || x.Price >= MinPrice) &&
                (MaxPrice == null || x.Price <= MaxPrice)
            );
        }

        
    }
}
