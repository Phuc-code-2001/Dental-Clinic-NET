using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Contacts;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class ContactAutoMapperProfile : Profile
    {
        public ContactAutoMapperProfile()
        {
            CreateMap<CreateContact, Contact>();
            CreateMap<Contact, ContactDTO>()
                .ForMember(des => des.State, act => act.MapFrom(src => src.State.ToString()));
        }
    }
}
