using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public class Post : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public BaseUser Creator { get; set; }
        public DateTime PublishDate { get; set; }

        public List<Service> Services { get; set; }
    }
}
