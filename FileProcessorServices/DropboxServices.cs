using Dropbox.Api;
using Dropbox.Api.Files;
using Dropbox.Api.Sharing;
using FileProcessorServices.Models;
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
        private static string refresh_token = "zcU4yYIK_zgAAAAAAAAAAbB1Ire9aglj18lPjY0GW6mLGcgg58b6GCSAkaI23sTr";
        private static string access_token = "sl.BSVVu20st9VNdmZnwaFMYZ2F4P1_JMzCyk4TP_SHcsGPEkJr7S47ByC1m2eKmrrBxeZR77tGFmAbxsoNK79MMXR4lFSo0DbJEzboHXZL6PJtRabg3lTQiCBN7uc_k1imlrNlLLE9w1D3";

        DropboxClient _dropBoxClient;

        string app_key = "bdccpj1xfgmkxzg";
        string app_secret = "sws2qqn87npvfce";


        public DropboxServices()
        {
            var config = new DropboxClientConfig();
            _dropBoxClient = new DropboxClient(access_token, refresh_token, app_key, app_secret, config);
        }

        public async Task<string> TestService(IFormFile file)
        {

            Stream stream = file.OpenReadStream();

            FileMetadata result = await _dropBoxClient.Files
                .UploadAsync("/" + file.FileName, mode: WriteMode.Add.Instance, body: stream);

            return await GetShareLinkAsync(result.PathDisplay);

        }

        public async Task<string> GetShareLinkAsync(string path)
        {

            try
            {
                var shareLinks = await _dropBoxClient.Sharing.ListSharedLinksAsync(path: path);
                if(shareLinks.Links.Count > 0)
                {
                    string url = shareLinks.Links[0].Url;
                    return url;
                }
                else
                {
                    var shareLink = await _dropBoxClient.Sharing.CreateSharedLinkWithSettingsAsync(path);
                    return shareLink.Url;
                }
            }
            catch(Exception ex)
            {

            }

            return null;
        }

        public async Task<DropBoxUploadResult> UploadAsync(IFormFile file, string filename)
        {
            
            try
            {
                Stream stream = file.OpenReadStream();
                FileMetadata result = await _dropBoxClient.Files
                    .UploadAsync("/" + filename, body: stream);

                string path = result.PathDisplay;
                string url = await GetShareLinkAsync(path);

                stream.Dispose();

                return new DropBoxUploadResult()
                {
                    UploadPath = path,
                    TemporaryURL = url,
                };

            }
            catch(Exception ex)
            {
                throw new Exception("Something went wrong: Upload error!");
            }
        }

    }
}
