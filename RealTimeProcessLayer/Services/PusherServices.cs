using DataLayer.DataContexts;
using DataLayer.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public delegate void CallBack(ITriggerResult result);

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

        public async Task PushTo(string[] channels, string actionName, object data, CallBack callBack)
        {
            string json_data = JsonConvert.SerializeObject(data, settings: new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                Formatting = Formatting.Indented
            });

            ITriggerResult result = await TriggerAsync(channels, actionName, json_data);
            callBack(result);
        }
    }
}
