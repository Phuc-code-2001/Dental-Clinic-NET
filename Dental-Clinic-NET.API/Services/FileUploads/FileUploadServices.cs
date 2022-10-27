using DataLayer.DataContexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DataLayer.Domain;

namespace Dental_Clinic_NET.API.Services.FileUploads
{
    public class FileUploadServices
    {
        IWebHostEnvironment _env;

        public FileUploadServices(IWebHostEnvironment env)
        {
            _env = env;
        }

        public FileUploadResult Upload(IFormFile file, string saveto = "Files")
        {

            FileUploadResult result = new FileUploadResult()
            {
                Succeeded = false
            };

            try
            {
                string filename = $"{DateTime.Now.Ticks}_{file.FileName}";
                string fileURL = Path.Combine(saveto, filename);
                string filepath = Path.Combine(_env.ContentRootPath, fileURL);

                string folder = Path.GetDirectoryName(filepath);
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            

                using (var fileStream = new FileStream(filepath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                result = new FileUploadResult()
                {
                    Succeeded = true,
                    FileName = filename,
                    FilePath = filepath,
                    FileUrl = fileURL
                };

                return result;
            }
            catch(Exception)
            {
                return result;
            }
            
        }
    }
}
