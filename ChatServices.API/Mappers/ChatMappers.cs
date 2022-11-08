using AutoMapper;
using ChatServices.API.DTOs;
using ChatServices.API.Models;
using DataLayer.Domain;

namespace ChatServices.API.Mappers
{
    public class ChatMappers : Profile
    {
        public ChatMappers()
        {
            CreateMap<PatToRecMessage, ChatMessage>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => Base64Encode(src.Content)));

            CreateMap<BaseUser, ChatUserDTO>();

            CreateMap<ChatMessage, ChatMessageDTO>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => Base64Decode(src.Content)));

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
