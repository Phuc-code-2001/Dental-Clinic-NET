using ImageProcessLayer.ImageKitResult;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessLayer.Services
{
    public interface IImageKitServices
    {
        public Task<ImageKitUploadResult> UploadImageAsync(byte[] file, string filename);
        public Task<ImageKitUploadResult> UploadImageAsync(IFormFile file, string filename);
        public Task DeleteImageAsync(string ImageId);
    }
}
