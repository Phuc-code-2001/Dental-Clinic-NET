using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SegementationXRayServices.Requests
{
    public class PredictionRequest
    {
        public string FileName { get; set; }
        public byte[] ImageInputData { get; set; }
        public string Purpose { get; set; }

        public PredictionRequest(string fileName, byte[] imageInputData, string purpose)
        {
            FileName = fileName;
            ImageInputData = imageInputData;
            Purpose = purpose;
        }

        public PredictionRequest(IFormFile file, string purpose = "")
        {
            FileName = file.FileName;
            using(var mStream = new MemoryStream())
            {
                file.CopyTo(mStream);
                byte[] data = mStream.ToArray();
                ImageInputData = data;
            }
            Purpose = purpose;
        }
    }
}
