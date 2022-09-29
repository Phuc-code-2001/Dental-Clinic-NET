using AutoMapper;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Services.Users;
using ImageProcessLayer.Services;

namespace Dental_Clinic_NET.API.Services
{
    public class ServicesManager
    {

        public IMapper AutoMapper { get; set; }

        public UserServices UserServices { get; set; }
        public FacebookServices FacebookServices { get; set; }
        public ImageKitServices ImageKitServices { get; set; }

        public ServicesManager(IMapper autoMapper, UserServices userServices, FacebookServices facebookServices, ImageKitServices imageKitServices)
        {
            AutoMapper = autoMapper;
            UserServices = userServices;
            FacebookServices = facebookServices;
            ImageKitServices = imageKitServices;
        }
        

    }
}
