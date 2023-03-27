using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegementationXRayServices.Responses
{
    public class ImageResult
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
