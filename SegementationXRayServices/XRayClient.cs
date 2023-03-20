using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RestSharp;
using SegementationXRayServices.Requests;
using SegementationXRayServices.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegementationXRayServices
{
    public class XRayClient
    {

        private const string LocalBaseURL = "http://127.0.0.1:8000/api";
        private RestClient _client;

        public XRayClient()
        {
            _client = new RestClient(LocalBaseURL);
        }
        public XRayClient(string baseURL)
        {
            _client = new RestClient(baseURL);
        }

        public XRayClient(IConfiguration configuration) : this(configuration["MechineLearningServer:BaseURL"])
        {
            
        }

        public async Task<SegmentationResponseData> UploadFileAsync(PredictionRequest data)
        {
            // process the response

            var request = new RestRequest("/predict/", Method.Post);
            

            request.AddFile("input_image", data.ImageInputData, data.FileName);
            //request.AddJsonBody(jsonData);
            //request.AddHeader("Content-Type", "multipart/form-data");

            
            request.AddParameter("purpose", data.Purpose, ParameterType.RequestBody);

            RestResponse response = await _client.ExecuteAsync(request);
            if(response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                string content = response.Content;
                SegmentationResponseData responseSuccessData = 
                    JsonConvert.DeserializeObject<SegmentationResponseData>(content);

                return responseSuccessData;
            }

            throw new Exception("Mechine learning API cannot complete this operation!");
        }


    }
}
