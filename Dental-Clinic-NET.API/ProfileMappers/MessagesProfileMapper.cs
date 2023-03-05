using AutoMapper;
using DataLayer.Domain;
using DataLayer.Extensions;
using Dental_Clinic_NET.API.DTOs.Messages;
using Dental_Clinic_NET.API.Models.Chats;

namespace Dental_Clinic_NET.API.ProfileMappers
{
    public class MessagesProfileMapper : Profile
    {
        public MessagesProfileMapper()
        {
            CreateMap<PatToRecMessage, Message>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => Base64Encode(src.Content)));

            CreateMap<RecToPatMessage, Message>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => Base64Encode(src.Content)))
                .ForMember(des => des.ToId, opt => opt.MapFrom(src => src.PatientId));

            CreateMap<BaseUser, ChatUserDTO>()
                .AfterMap((src, des) =>
                {
                    des.UserRole = src.Type.ToString();
                });

            CreateMap<Message, ChatMessageDTO>()
                .ForMember(des => des.Content, opt => opt.MapFrom(src => src.IsRemoved ? string.Empty : Base64Decode(src.Content)));

            CreateMap<Conversation, ConversationDTO>()
                .ForMember(des => des.LastMessageCreated, opt => opt.MapFrom(src => src.LastMessage.TimeCreated.Value))
                .AfterMap((src, des) =>
                {
                    des.PreviewContent = src.LastMessage.Content.Substring(0, 15) + "...";
                    des.TimeFormatted = TimeManager.TranslateTimeToAgo(src.LastMessage.TimeCreated.Value);
                });


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
