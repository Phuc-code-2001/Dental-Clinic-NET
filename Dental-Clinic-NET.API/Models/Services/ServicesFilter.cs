using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Services
{
    public class ServicesFilter : PageFilter
    {
        public string Code { get; set; }
        public string Name { get; set; }

        public int MinPrice { get; set; } = int.MinValue;
        public int MaxPrice { get; set; } = int.MaxValue;

        public bool Union { get; set; } = false;

        public IQueryable<Service> GetFilteredQuery(IQueryable<Service> src)
        {
            bool hasFilterRequired = Code != null && Name != null && MinPrice != int.MinValue && MaxPrice != int.MaxValue;
            if(hasFilterRequired)
            {
                return src.AsEnumerable().Where(service => CheckConditionService(service)).AsQueryable();
            }
            return src;
        }

        private bool CheckConditionService(Service service)
        {
            bool rootCondition = !Union;

            if (Union)
            {
                if (!string.IsNullOrWhiteSpace(Code))
                {
                    rootCondition = rootCondition || service.ServiceCode.Contains(Code);
                }

                if (!string.IsNullOrWhiteSpace(Name))
                {
                    rootCondition = rootCondition || service.ServiceName.Contains(Name);
                }

                rootCondition = rootCondition || (MinPrice <= service.Price && service.Price <= MaxPrice);

            }
            else
            {
                if (!string.IsNullOrWhiteSpace(Code))
                {
                    rootCondition = rootCondition && service.ServiceCode.Contains(Code);
                }

                if (!string.IsNullOrWhiteSpace(Name))
                {
                    rootCondition = rootCondition && service.ServiceName.Contains(Name);
                }

                rootCondition = rootCondition && (MinPrice <= service.Price && service.Price <= MaxPrice);
            }

            return rootCondition;
        }

    }
}
