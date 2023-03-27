using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegementationXRayServices.Responses
{
    public class SegmentationResponseData
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("instance_id")]
        public string InstanceId { get; set; }
        [JsonProperty("module_name")]
        public string ModuleName { get; set; }
        [JsonProperty("input_image")]
        public string InputImage { get; set; }
        [JsonProperty("prediction_result_set")]
        public List<PredictionResult> PredictionResultSet { get; set; }
        [JsonProperty("purpose")]
        public string Purpose { get; set; }
        [JsonProperty("upload_at")]
        public DateTime UploadAt { get; set; }
    }
}
