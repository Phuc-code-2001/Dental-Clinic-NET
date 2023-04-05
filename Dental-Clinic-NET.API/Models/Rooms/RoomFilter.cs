using DataLayer.Domain;
using Dental_Clinic_NET.API.Utils;
using System.Linq;

namespace Dental_Clinic_NET.API.Models.Rooms
{
    public class RoomFilter : PageFilter
    {
        public int? RoomId { get; set; }
        public string Code { get; set; }
        public int? RoomTypeId { get; set; }
        public int? CategoryId { get; set; }

        public IQueryable<Room> GetFilteredQuery(IQueryable<Room> source)
        {
            return source.Where(x =>
                (RoomId == null || x.Id == RoomId) &&
                (RoomTypeId == null || (int) x.RoomType == RoomTypeId) &&
                (CategoryId == null || x.RoomCategory.Id == CategoryId) &&
                (!string.IsNullOrWhiteSpace(Code) || x.RoomCode.Contains(Code))
            );
        }

    }
}
