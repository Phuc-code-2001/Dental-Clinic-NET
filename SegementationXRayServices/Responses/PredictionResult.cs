using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegementationXRayServices.Responses
{
    public class PredictionResult
    {
        [JsonProperty("image_result_set")]
        public List<ImageResult> ImageResultSet { get; set; }
        [JsonProperty("teeth_count")]
        public int TeethCount { get; set; }
    }
}
