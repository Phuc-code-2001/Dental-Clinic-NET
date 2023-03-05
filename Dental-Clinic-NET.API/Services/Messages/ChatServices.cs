using DataLayer.DataContexts;

namespace Dental_Clinic_NET.API.Services.Messages
{
    public class ChatServices
    {
        AppDbContext DbContext { get; set; }

        public ChatServices(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }



    }
}
