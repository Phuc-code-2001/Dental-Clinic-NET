using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Contacts
{
    public class ContactFilter : PageFilter
    {
        public DateTime From { get; set; } = DateTime.MinValue;
        public DateTime To { get; set; } = DateTime.MaxValue;

        public ContactStates? State { get; set; }

        public string Keyword { get; set; }

        public IQueryable<Contact> GetFilterdQuery(IQueryable<Contact> src)
        {
            if(!string.IsNullOrWhiteSpace(Keyword))
            {
                Keyword = Keyword.Trim();
                src = src.Where(x => x.Id.ToString().Contains(Keyword) || x.Name.Contains(Keyword) || x.PhoneNumber.Contains(Keyword));
            }

            return src.Where(x => (State == null || x.State == State) && x.TimeCreated.Value.Date >= From && x.TimeCreated.Value.Date <= To);
        }

    }
}
