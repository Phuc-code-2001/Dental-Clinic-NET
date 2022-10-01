using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Domain
{
    public abstract class BaseEntity
    {
        public DateTime? TimeCreated { get; set; } = DateTime.Now;
        public DateTime? LastTimeModified { get; set; }
    }
}
