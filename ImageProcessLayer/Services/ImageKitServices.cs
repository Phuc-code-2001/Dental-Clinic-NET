using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imagekit;
using ImageProcessLayer.ImageKitResult;
using Microsoft.AspNetCore.Http;

namespace ImageProcessLayer.Services
{
    public class ImageKitServices : IImageKitServices
    {

        private ServerImagekit imagekit;
        private string publicKey = "public_aUyELt2YVwpWP00c2S97uv2X2ps";
        private string privateKey = "private_/ruWnqNpOopwwW6SAQxUxNjADLA=";

        public string URLEndPoint = "https://ik.imagekit.io/sdrpji7cj/dental-clinic-management";

        private int MaxWidth = 512;
        private int MaxHeight = 512;

        public ImageKitServices()
        {
            imagekit = new ServerImagekit(publicKey, privateKey, URLEndPoint);
        }

        public async Task<ImageKitUploadResult> UploadImageAsync(byte[] file, string filename)
        {
            Transformation transformation = new Transformation().Width(MaxWidth).Height(MaxHeight);
            ImagekitResponse resp = await imagekit.Url(transformation).FileName(filename).UploadAsync(file);
            return new ImageKitUploadResult() { ImageId=resp.FileId, URL=resp.URL };
        }

        public async Task DeleteImageAsync(string ImageId)
        {
            try
            {
                _ = await imagekit.DeleteFileAsync(ImageId);
            }
            catch(Exception ex)
            {
                throw new Exception($"Something went wrong: {ex.Message}");
            }
        }

        public async Task<ImageKitUploadResult> UploadImageAsync(IFormFile file, string filename)
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();

                    ImageKitUploadResult result = await UploadImageAsync(fileBytes, filename);
                    return result;
                }
            }

            throw new Exception("Something went wrong while upload image");
        }

        public bool IsImage(IFormFile file)
        {
            if (file == null) return false;
            if (file.ContentType.StartsWith("image")) return true;
            return false;
        }
    }
}
