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
            using(var ms = new MemoryStream())
            {
                await file.CopyToAsync(ms);
                var response = await _dropBoxClient.Files
                    .UploadAsync(_appFolder, WriteMode.Overwrite.Instance, body: ms);

                result = response.PreviewUrl;
            }

            return result;
        }

        public string Upload(IFormFile file, string filename)
        {
            

            return "";
        }

    }
}
