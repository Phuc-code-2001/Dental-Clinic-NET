using Dropbox.Api;
using Dropbox.Api.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileProcessorServices
{
    public class DropboxServices
    {
        IConfiguration Configuration;
        DropboxClient _dropBoxClient;
        string _appFolder;


        public DropboxServices(IConfiguration configuration)
        {
            Configuration = configuration;
            _appFolder = Configuration["Dropbox:APP_FOLDER"];
            _dropBoxClient = new DropboxClient(Configuration["Dropbox:ACCESS_TOKEN"]);

        }


        public async Task<string> TestService(IFormFile file)
        {

            string result = "";
            using (var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                byte[] bytes = ms.ToArray();

                var response = await _dropBoxClient.Files
                    .UploadAsync("/" + file.FileName, WriteMode.Overwrite.Instance, body: ms);

                result = $"Uploaded Id {response.Id} Rev {response.Rev}";
            }

            return result;
        }

        public string Upload(IFormFile file, string filename)
        {
            

            return "";
        }

    }
}
