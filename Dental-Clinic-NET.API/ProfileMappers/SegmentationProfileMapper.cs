using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class SegmentationProfileMapper : Profile
    {
        public SegmentationProfileMapper()
        {
            CreateMap<SegmentationResult, SegmentationResultDTO>();
            CreateMap<SegmentationResult.SegmentationImageResult, SegmentationResultDTO.SegmentationImageResultDTO>();

            CreateMap<SegmentationResult, SegmentationResultDTOLite>()
                .ForMember(des => des.Technician, opt => opt.MapFrom(src => src.Technican.UserName));
            CreateMap<SegmentationResult.SegmentationImageResult, SegmentationResultDTOLite.SegmentationImageResultDTOLite>();
        }
    }
}
