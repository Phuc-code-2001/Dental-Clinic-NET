using AutoMapper;
using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Serializers;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using ImageProcessLayer.ImageKitResult;
using ImageProcessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using RealTimeProcessLayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private ServicesManager _servicesManager;

        public HelperController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateChannelIfNullAsync()
        {
            var users = _servicesManager.UserManager.Users.ToList();
            foreach(var user in users)
            {
                if (String.IsNullOrEmpty(user.PusherChannel))
                {
                    string channel = _servicesManager.UserServices.GenerateUniqueUserChannel();
                    user.PusherChannel = channel;
                    await _servicesManager.UserManager.UpdateAsync(user);
                }
            }

            var userDTOs = users.Select(user => _servicesManager.AutoMapper.Map<UserDTO>(user)).ToList();

            return Ok(userDTOs);

        }

        [HttpPost]
        public async Task<IActionResult> TestPostImageAsync(IFormFile file)
        {
            try
            {
                bool validFileImage = _servicesManager.ImageKitServices.IsImage(file);
                if (!validFileImage) return BadRequest("Input must be a image");

                string filename = file.FileName;
                ImageKitUploadResult result = await _servicesManager.ImageKitServices.UploadImageAsync(file, filename);

                return Ok(result);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete]
        public async Task<IActionResult> TestDeleteImageAsync(string imageId)
        {
            try
            {
                
                await _servicesManager.ImageKitServices.DeleteImageAsync(imageId);
                return Ok($"Delete image with id={imageId} success");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [EnableQuery]
        public IActionResult ViewAllAccount(int page = 1)
        {
            try
            {

                var users = _servicesManager.UserManager.Users.AsEnumerable().Select(user =>
                {
                    PermissionOnBaseUser permission = new PermissionOnBaseUser(user, user);
                    UserSerializer serializer = new UserSerializer(permission);
                    return serializer.Serialize(user =>
                    {
                        return _servicesManager.AutoMapper.Map<UserDTO>(user);
                    });
                });

                
                Paginated<UserDTO> paginatedUsers = new Paginated<UserDTO>(users.AsQueryable(), page);
                
                return Ok(new
                {
                    page=page,
                    per_page=paginatedUsers.PageSize,
                    total=paginatedUsers.QueryCount,
                    total_pages=paginatedUsers.PageCount,
                    data=paginatedUsers.Items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public IActionResult GetPatients()
        {
            var results = _servicesManager.DbContext.Patients.Include(pat => pat.BaseUser).Include(pat => pat.MedicalRecordFile)
                .Select(pat => _servicesManager.AutoMapper.Map<PatientDTO>(pat)).ToList();

            return Ok(results);
        }

        [HttpGet]
        public IActionResult GetFiles()
        {
            var results = _servicesManager.DbContext.FileMedias.Select(file => _servicesManager.AutoMapper.Map<MediaFileDTO>(file)).ToList();
            return Ok(results);
        }

        [HttpPost]
        public IActionResult GeneratePatientProfiles()
        {
            try
            {
                var users = _servicesManager.UserManager.Users.ToList();
                int count = 0;
                users.ForEach(user =>
                {
                    Patient patient = _servicesManager.DbContext.Patients.Find(user.Id);
                    if (patient == null)
                    {
                        patient = new Patient()
                        {
                            Id = user.Id,
                            MedicalRecordFile = new FileMedia() { Category = FileMedia.FileCategory.MedicalRecord },
                        };
                        _servicesManager.DbContext.Patients.Add(patient);
                        count++;
                    }

                });

                _servicesManager.DbContext.SaveChanges();

                return Ok($"Mới tạo được {count} thằng.");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> TestDropbox(IFormFile file)
        {
            string result = await _servicesManager.DropboxServices.TestService(file);
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllDocuments()
        {
            var dataset = _servicesManager.DbContext.Documents
                .Include(d => d.File)
                .ToList();

            return Ok(dataset);
        }

        [HttpDelete]
        public IActionResult DeleteAllAppointment()
        {
            var dataset = _servicesManager.DbContext.Appointments.ToArray();
            _servicesManager.DbContext.Appointments.RemoveRange(dataset);
            _servicesManager.DbContext.SaveChanges();

            return Ok();
        }


        [HttpPost]
        public async Task<IActionResult> TestKickBox(string email)
        {
            var result = await _servicesManager.KickboxServices.VerifyEmailAsync(email);
            return Ok(result);
        }
    }
}
