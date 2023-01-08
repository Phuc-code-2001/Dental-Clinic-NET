using AutoMapper;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Services.Users;
using FileProcessorServices;
using ImageProcessLayer.Services;
using MailServices.Services;
using RealTimeProcessLayer.Services;

namespace Dental_Clinic_NET.API.Services
{
    public class ServicesManager
    {

        public IMapper AutoMapper { get; set; }

        public UserServices UserServices { get; set; }
        public FacebookServices FacebookServices { get; set; }
        public ImageKitServices ImageKitServices { get; set; }
        public PusherServices PusherServices { get; set; }

        public DropboxServices DropboxServices { get; set; }
        public KickboxServices KickboxServices { get; set; }

        public ServicesManager(IMapper autoMapper, UserServices userServices, FacebookServices facebookServices, ImageKitServices imageKitServices, PusherServices pusherServices, DropboxServices dropboxServices, KickboxServices kickboxServices)
        {
            AutoMapper = autoMapper;
            UserServices = userServices;
            FacebookServices = facebookServices;
            ImageKitServices = imageKitServices;
            PusherServices = pusherServices;
            DropboxServices = dropboxServices;
            KickboxServices = kickboxServices;
        }


    }
}
