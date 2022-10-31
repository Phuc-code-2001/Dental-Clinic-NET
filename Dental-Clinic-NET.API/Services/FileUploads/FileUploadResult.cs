using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Services.FileUploads
{
    public class FileUploadResult
    {
        public bool Succeeded { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileUrl { get; set; }
    }
}
