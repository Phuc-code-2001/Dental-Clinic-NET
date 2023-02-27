using AutoMapper;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Services.Appointments;
using Dental_Clinic_NET.API.Services.Doctors;
using Dental_Clinic_NET.API.Services.Notifications;
using Dental_Clinic_NET.API.Services.Users;
using FileProcessorServices;
using ImageProcessLayer.Services;
using MailServices;
using Microsoft.AspNetCore.Identity;
using RealTimeProcessLayer.Services;

namespace Dental_Clinic_NET.API.Services
{
    public class ServicesManager
    {

        public AppDbContext DbContext { get; set; }
        public UserManager<BaseUser> UserManager { get; set; }

        public IMapper AutoMapper { get; set; }

        public ImageKitServices ImageKitServices { get; set; }
        public PusherServices PusherServices { get; set; }

        public DropboxServices DropboxServices { get; set; }
        public KickboxServices KickboxServices { get; set; }
        public EmailSender EmailSender { get; set; }

        public UserServices UserServices { get; set; }
        public AppointmentServices AppointmentServices { get; set; }
        public DoctorServices DoctorServices { get; set; }

        public NotificationServices NotificationServices { get; set; }

        public ServicesManager(
            IMapper autoMapper,
            UserServices userServices,
            ImageKitServices imageKitServices,
            PusherServices pusherServices,
            DropboxServices dropboxServices,
            KickboxServices kickboxServices,
            EmailSender emailSender,
            AppDbContext dbContext,
            UserManager<BaseUser> userManager,
            AppointmentServices appointmentServices,
            DoctorServices doctorServices,
            NotificationServices notificationServices)
        {
            DbContext = dbContext;
            UserManager = userManager;
            AutoMapper = autoMapper;
            UserServices = userServices;
            ImageKitServices = imageKitServices;
            PusherServices = pusherServices;
            DropboxServices = dropboxServices;
            KickboxServices = kickboxServices;
            EmailSender = emailSender;
            AppointmentServices = appointmentServices;
            DoctorServices = doctorServices;
            NotificationServices = notificationServices;
        }


    }
}
