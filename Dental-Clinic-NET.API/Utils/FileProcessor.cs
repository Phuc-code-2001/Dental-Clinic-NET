using DataLayer.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Utils
{
    public class FileProcessor
    {
		/// <summary>
		///		Save a form file to server
		/// </summary>
		/// <param name="fileUpload">File to save</param>
		/// <param name="savePath">Relative path</param>
		/// <returns>Part URL of given file</returns>
        public static async Task<string> SaveToAsync(IFormFile fileUpload, string savePath)
		{
			try
            {
				string filename = $"{DateTime.Now.Ticks}_{fileUpload.FileName}";
				string RelativeURL = Path.Combine(savePath, filename);
				var filepath = Path.Combine(Directory.GetCurrentDirectory(), RelativeURL);

				var folder = Path.GetDirectoryName(filepath);
				if (!Directory.Exists(folder))
				{
					Directory.CreateDirectory(folder);
				}

				using (var fileStream = new FileStream(filepath, FileMode.Create))
				{
					await fileUpload.CopyToAsync(fileStream);
				}

				return RelativeURL;
            }
			catch(Exception ex)
            {
				throw new Exception("File upload failed: " + ex.Message);
            }
		}
	}
}
