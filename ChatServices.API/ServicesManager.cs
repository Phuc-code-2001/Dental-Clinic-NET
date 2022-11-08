using AutoMapper;
using DataLayer.DataContexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealTimeProcessLayer.Services;

namespace ChatServices.API
{
    public class ServicesManager
    {
        public AppDbContext DbContext { get; set; }
        public PusherServices PusherServices { get; set; }
        public IMapper AutoMapper { get; set; }

        public ServicesManager(AppDbContext dbContext, PusherServices pusherServices, IMapper autoMapper)
        {
            DbContext = dbContext;
            PusherServices = pusherServices;
            AutoMapper = autoMapper;
        }
    }
}
