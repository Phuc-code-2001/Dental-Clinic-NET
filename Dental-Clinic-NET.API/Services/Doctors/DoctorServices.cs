using DataLayer.Domain;
using FileProcessorServices;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Services.Doctors
{
    public class DoctorServices
    {

        DropboxServices _dropboxServices;

        public DoctorServices(DropboxServices dropboxServices)
        {
            _dropboxServices = dropboxServices;
        }

        public async Task<FileMedia> UploadCertificateAsync(IFormFile file, string filename)
        {
            if (file.ContentType.EndsWith("pdf"))
            {
                var uploadResult = await _dropboxServices.UploadAsync(file, filename);

                FileMedia certificate = new FileMedia()
                {
                    FilePath = uploadResult.UploadPath,
                    Category = FileMedia.FileCategory.DoctorCertificate,
                };

                return certificate;
            }
            else
            {
                throw new Exception("File format must be *.pdf");
            }
        }

    }
}
