using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Chats
{
    public class ConversationFilter
    {
        public int? Take { get; set; } = 10;
        public int? Skip { get; set; } = 0;
        public string UserName { get; set; }

        public IQueryable<Conversation> GetFilteredData(IQueryable<Conversation> source) 
        {
            return source
                .Where(x => string.IsNullOrWhiteSpace(UserName) || x.User.UserName.Contains(UserName))
                .OrderByDescending(x => x.HasMessageUnRead).ThenByDescending(x => x.LastMessageId)
                .Skip(Skip.Value).Take(Take.Value);
        }
    }
}
