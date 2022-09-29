using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProcessLayer.Models
{
    public class PusherMessage<T>
    {
        public PusherTypes Type;
        public T Payload;
    }
}
