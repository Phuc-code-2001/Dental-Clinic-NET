using AutoMapper;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RealTimeProcessLayer.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Services.Notifications
{
    public class NotificationServices
    {
        AppDbContext DbContext { get; set; }
        PusherServices PusherServices { get; set; }
        IMapper Mapper { get; set; }

        public NotificationServices(AppDbContext dbContext, PusherServices pusherServices, IMapper mapper)
        {
            DbContext = dbContext;
            PusherServices = pusherServices;
            Mapper = mapper;
        }

        public IQueryable<Notification> QueryAll()
        {
            return DbContext.Notifications.Include(notify => notify.Receiver);
        }

        public void SendToClient(Notification obj)
        {
            string[] channels = new string[]
            {
                obj.Receiver.PusherChannel,
            };

            NotificationDTO dto = Mapper.Map<NotificationDTO>(obj);
            _ = PusherServices.PushToAsync(channels, "Notification", dto);
        }

    }

}
