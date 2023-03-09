using DataLayer.DataContexts;
using DataLayer.Domain;
using FileProcessorServices;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Services.Doctors
{
    public class DoctorServices
    {

        DropboxServices _dropboxServices;
        AppDbContext DbContext;

        public DoctorServices(DropboxServices dropboxServices, AppDbContext dbContext)
        {
            _dropboxServices = dropboxServices;
            DbContext = dbContext;
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

        public async Task<Doctor> GetOrCreateDoctorInfoAsync(string userId)
        {
            BaseUser user = await DbContext.Users
                .FirstOrDefaultAsync(x => x.Id == userId && x.Type == UserType.Doctor);

            if (user == null)
            {
                return null;
            }

            Doctor doctor = await DbContext.Doctors.FirstOrDefaultAsync(x => x.Id == userId);
            if(doctor == null)
            {
                doctor = new Doctor()
                {
                    Id = userId,
                    Certificate = new FileMedia(),
                    Major = "Wait for update",
                    Verified = true,
                };

                DbContext.Doctors.Add(doctor);
                DbContext.SaveChanges();
            }

            return doctor;
        }

    }
}
