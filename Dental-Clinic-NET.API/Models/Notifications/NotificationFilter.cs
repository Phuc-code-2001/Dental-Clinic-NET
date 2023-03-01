using DataLayer.Domain;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Dental_Clinic_NET.API.Models.Notifications
{
    public class NotificationFilter
    {
        public string ReceiverId { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string Keyword { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

        public Expression<Func<Notification, bool>> ExprReceiver()
        {
            return obj => string.IsNullOrWhiteSpace(ReceiverId) || obj.Receiver.Id == ReceiverId;
        }

        public Expression<Func<Notification, bool>> ExprFrom()
        {
            return obj => !From.HasValue || From.Value >= obj.TimeCreated;
        }

        public Expression<Func<Notification, bool>> ExprTo()
        {
            return obj => !To.HasValue || To.Value <= obj.TimeCreated;
        }

        public Expression<Func<Notification, bool>> ExprKeyword()
        {
            return obj => string.IsNullOrWhiteSpace(Keyword) || obj.Content.Contains(Keyword.Trim());
        }

        public IQueryable<Notification> Filter(IQueryable<Notification> src)
        {
            return src.Where(ExprReceiver())
                .Where(ExprFrom())
                .Where(ExprTo())
                .Where(ExprKeyword());
        }
        
    }
}
