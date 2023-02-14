using AutoMapper;
using ChatServices.API.DTOs;
using ChatServices.API.Models;
using DataLayer.Domain;

namespace ChatServices.API.Mappers
{
    public class ChatMapperProfile : Profile
    {
        public ChatMapperProfile()
        {
            CreateMap<PatToRecMessage, ChatMessage>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => Base64Encode(src.Content)));

            CreateMap<RecToPatMessage, ChatMessage>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => Base64Encode(src.Content)))
                .ForMember(des => des.ToId, opt => opt.MapFrom(src => src.PatientId));

            CreateMap<BaseUser, ChatUserDTO>();

            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(des => des.Content, opt => opt.MapFrom(
                    src => src.IsRemoved ? string.Empty : Base64Decode(src.Content)));

            CreateMap<UserInChatBoxOfReception, UserInChatBoxOfReceptionDTO>()
                .ForMember(des => des.LastMessageCreated, opt => opt.MapFrom(src => src.LastMessage.TimeCreated));


        }

        public static string Base64Encode(string text)
        {
            var textBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(textBytes);
        }

        public static string Base64Decode(string base64)
        {
            var base64Bytes = System.Convert.FromBase64String(base64);
            return System.Text.Encoding.UTF8.GetString(base64Bytes);
        }
    }
}
