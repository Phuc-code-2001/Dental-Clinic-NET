using DataLayer.DataContexts;
using DataLayer.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RealTimeProcessLayer.Services;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dental_Clinic_NET.API.Services.Notifications
{
    public class NotificationServices
    {
        AppDbContext DbContext { get; set; }
        PusherServices PusherServices { get; set; }

        public NotificationServices(AppDbContext dbContext, PusherServices pusherServices)
        {
            DbContext = dbContext;
            PusherServices = pusherServices;
        }

        public IQueryable<Notification> QueryAll()
        {
            return DbContext.Notifications.Include(notify => notify.Receiver);
        }

        public bool Send(Notification obj)
        {
            return true;
        }

    }

}
