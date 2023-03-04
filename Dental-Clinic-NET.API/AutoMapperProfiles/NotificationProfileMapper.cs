using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using System;
using System.Linq.Expressions;
using System.Text;

namespace Dental_Clinic_NET.API.AutoMapperProfiles
{
    public class NotificationProfileMapper : Profile
    {
        public NotificationProfileMapper()
        {
            CreateMap<Notification, NotificationDTO>()
                .ForMember(des => des.CreatedFormated, opt => opt.MapFrom(src => TranslateTimeSpan(DateTime.Now - src.TimeCreated.Value)))
                .ForMember(des => des.Category, opt => opt.MapFrom(src => src.Category.ToString()));
        }

        private static string TranslateTimeSpan(TimeSpan duration)
        {
            int totalSeconds = (int) Math.Round(duration.TotalSeconds, 0);

            int minuteUnit = 60;
            int hourUnit = 60 * minuteUnit;
            int dayUnit = 24 * hourUnit;

            string message = "";
            int dayCount = totalSeconds / dayUnit;
            if (dayCount > 0)
            {
                message += String.Format("{0} day{1} ", dayCount, dayCount > 1 ? "s": "");
                totalSeconds %= dayUnit;
            }
            else
            {
                int hourCount = totalSeconds / hourUnit;
                if (hourCount > 0)
                {
                    message += String.Format("{0} hour{1} ", hourCount, hourCount > 1 ? "s" : "");
                    totalSeconds %= hourUnit;
                }

                int minuteCount = totalSeconds / minuteUnit;
                if (minuteCount > 0)
                {
                    message += String.Format("{0} minute{1} ", minuteCount, minuteCount > 1 ? "s" : "");
                    totalSeconds %= minuteUnit;
                }

                if(hourCount == 0)
                {
                    if (totalSeconds > 0)
                    {
                        message += String.Format("{0} second{1} ", totalSeconds, totalSeconds > 1 ? "s" : "");
                    }
                }

            }

            message += "ago";

            return message;
        }
    }
}
