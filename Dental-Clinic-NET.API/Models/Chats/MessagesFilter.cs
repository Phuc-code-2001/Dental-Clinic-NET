using DataLayer.Domain;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Chats
{
    public class MessagesFilter
    {
        public int Take { get; set; }
        public int Skip { get; set; }

        public string Text { get; set; }

        public IQueryable<Message> GetData(IQueryable<Message> src)
        {
            if(!string.IsNullOrWhiteSpace(Text))
            {
                src = src.Where(x => x.Content.Contains(Text));
            }

            return src.Skip(Skip).Take(Take);
        }

    }
}
