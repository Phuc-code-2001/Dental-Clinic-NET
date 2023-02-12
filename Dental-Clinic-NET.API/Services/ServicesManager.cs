using AutoMapper;
using Dental_Clinic_NET.API.Services.Users;
using FileProcessorServices;
using ImageProcessLayer.Services;
using MailServices;
using RealTimeProcessLayer.Services;

namespace Dental_Clinic_NET.API.Services
{
    public class ServicesManager
    {

        public IMapper AutoMapper { get; set; }

        public UserServices UserServices { get; set; }
        public ImageKitServices ImageKitServices { get; set; }
        public PusherServices PusherServices { get; set; }

        public DropboxServices DropboxServices { get; set; }
        public KickboxServices KickboxServices { get; set; }
        public EmailSender EmailSender { get; set; }

        public ServicesManager(IMapper autoMapper, UserServices userServices, ImageKitServices imageKitServices, PusherServices pusherServices, DropboxServices dropboxServices, KickboxServices kickboxServices, EmailSender emailSender)
        {
            AutoMapper = autoMapper;
            UserServices = userServices;
            ImageKitServices = imageKitServices;
            PusherServices = pusherServices;
            DropboxServices = dropboxServices;
            KickboxServices = kickboxServices;
            EmailSender = emailSender;
        }


    }
}
