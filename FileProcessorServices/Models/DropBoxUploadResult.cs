using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessorServices.Models
{
    public class DropBoxUploadResult
    {
        public string UploadPath { get; set; }
        public string TemporaryURL { get; set; }
    }
}
