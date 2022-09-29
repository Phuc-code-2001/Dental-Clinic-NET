using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PusherServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProcessLayer.Services
{
    public class PusherServices : Pusher, IPusherServices
    {

        public PusherServices(IConfiguration configuration) 
        : base(
                configuration["Pusher:AppId"], 
                configuration["Pusher:Key"], 
                configuration["Pusher:Secret"], 
                new PusherOptions
                {
                    Cluster = configuration["Pusher:Cluster"],
                    Encrypted = true
                }
        )
        {
            

            
            
        }

        
        
    }
}
