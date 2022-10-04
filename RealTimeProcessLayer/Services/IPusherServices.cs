using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RealTimeProcessLayer.Services.PusherServices;

namespace RealTimeProcessLayer.Services
{
    public interface IPusherServices
    {
        public string GenerateUniqueUserChannel();
        public Task PushTo(string[] channels, string actionName, object data, CallBack callBack);
    }
}
