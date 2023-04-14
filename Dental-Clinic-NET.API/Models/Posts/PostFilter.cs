using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Posts
{
    public class PostFilter : PageFilter
    {
        public string Creator { get; set; }
        public string Title { get; set; }
        public DateTime? StartAt { get; set; } = DateTime.MinValue;
        public DateTime? EndAt { get; set; } = DateTime.MaxValue;

        public IQueryable<Post> GetFilteredQuery(IQueryable<Post> source)
        {
            return source.Where(x =>
                (string.IsNullOrWhiteSpace(Creator) || x.Creator.UserName.Contains(Creator)) &&
                (string.IsNullOrWhiteSpace(Title) || x.Title.Contains(Title)) &&
                (x.TimeCreated >= StartAt) &&
                (x.TimeCreated <= EndAt)
            );
        }
    }
}
