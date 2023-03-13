using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imagekit;
using ImageProcessLayer.ImageKitResult;
using Microsoft.AspNetCore.Http;
using System.Drawing;
using System.Drawing.Drawing2D;

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
            ImagekitResponse resp = await imagekit.FileName(filename).UploadAsync(file);
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
                // Convert the image to PNG format
                var stream = file.OpenReadStream();
                var pngStream = ConvertToPng(stream);
                
                var fileBytes = pngStream.ToArray();
                string ext = ".png";
                ImageKitUploadResult result = await UploadImageAsync(fileBytes, filename + ext);
                return result;
                
            }

            throw new Exception("Something went wrong while upload image");
        }

        private MemoryStream ConvertToPng(Stream stream)
        {
            // Convert the image to PNG format
            var image = Image.FromStream(stream);
            // image = ResizeImage(image);
            var pngStream = new MemoryStream();
            image.Save(pngStream, System.Drawing.Imaging.ImageFormat.Png);
            pngStream.Position = 0;
            return pngStream;
        }

        private Image ResizeImage(Image image)
        {
            int targetWidth = MaxWidth;
            int targetHeight = MaxHeight;
            int width, height;

            // Determine the width and height of the image
            if (image.Width > image.Height)
            {
                width = targetWidth;
                height = (int)(image.Height * ((float)targetWidth / image.Width));
            }
            else
            {
                width = (int)(image.Width * ((float)targetHeight / image.Height));
                height = targetHeight;
            }

            // Create a new image with the target size
            Image newImage = new Bitmap(targetWidth, targetHeight);

            // Resize the image
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(image, 0, 0, width, height);
            }

            return newImage;
        }

        public bool IsImage(IFormFile file)
        {
            if (file == null) return false;
            if (file.ContentType.StartsWith("image")) return true;
            return false;
        }
    }
}
