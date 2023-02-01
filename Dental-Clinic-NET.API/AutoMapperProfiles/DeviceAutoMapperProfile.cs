using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Devices;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class DeviceAutoMapperProfile : Profile
    {
        public DeviceAutoMapperProfile()
        {
            CreateMap<CreateDevice, Device>();

            CreateMap<Room, DeviceDTO.RoomInnerDTO>();
            CreateMap<Service, DeviceDTO.ServiceInnerDTO>();
            CreateMap<Device, DeviceDTO>();
           
            CreateMap<UpdateDevice, Device>()
                .ForMember(des => des.Date, opt => opt.MapFrom(src => src.Date.Value))
                .ForMember(des => des.RoomId, opt => opt.MapFrom(src => src.RoomId.Value))
                .ForMember(des => des.DeviceValue, opt => opt.MapFrom(src => src.DeviceValue.Value))
                .ForMember(des => des.Status, opt => opt.MapFrom(src => src.Status.Value))
                .ForAllMembers(opt => opt.Condition((src, des, field) =>
                {
                    bool condition_01 = field != null && field is not string;
                    bool condition_02 = field is string && !string.IsNullOrWhiteSpace(field.ToString());
                    return condition_01 || condition_02;
                }));


            CreateMap<Device, EnumTypeDTO>()
                .ForMember(des => des.Name, opt => opt.MapFrom(src => src.DeviceName));

        }

        //private string ConvertImageFile(IFormFile formFile)
        //{
        //    ImageKitServices imageKitServices = new ImageKitServices();
        //    var result = imageKitServices.UploadImageAsync(formFile, formFile.FileName).Result;
        //    return result.URL;
        //}
        
    }
}
