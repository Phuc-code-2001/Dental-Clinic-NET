using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Patients;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Services.FileUploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private AppDbContext _context;
        private UserManager<BaseUser> _userManager;
        private ServicesManager _servicesManager;

        public PatientController(AppDbContext context, ServicesManager servicesManager, UserManager<BaseUser> userManager)
        {
            _context = context;
            _servicesManager = servicesManager;
            _userManager = userManager;
        }


        private IQueryable<Patient> FullyQueryPatientFromContext()
        {
            return _context.Patients.Include(pat => pat.BaseUser).Include(pat => pat.MedicalRecordFile);
        }

        /// <summary>
        ///     Get list patients of system
        /// </summary>
        /// <returns>
        ///     500: Server handle error
        ///     200: Success
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        [EnableQuery(PageSize = 10)]
        public IActionResult GetAll()
        {
            try
            {
                var patients = FullyQueryPatientFromContext()
                    .Select(pat => _servicesManager.AutoMapper.Map<PatientDTO>(pat)).ToList();
                return Ok(patients);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Get detail of patient
        /// </summary>
        /// <param name="id">Id of patient</param>
        /// <returns>
        ///     200: Get success
        ///     403: Forbiden
        ///     404: Not found
        ///     500: Server handle error
        /// </returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPatientByIdAsync(string id)
        {
            try
            {
                Patient patient = await FullyQueryPatientFromContext().FirstOrDefaultAsync(pat => pat.Id == id);
                if(patient == null)
                {
                    return NotFound("Truyền sai id rồi Hảo moiz");
                }

                BaseUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, patient.BaseUser);
                
                if(loggedUser.Type == UserType.Doctor || permission.IsOwner || permission.IsAdmin)
                {
                    PatientDTO patientDTO = _servicesManager.AutoMapper.Map<PatientDTO>(patient);
                    return Ok(patientDTO);
                }


                return StatusCode(403, "Đăng nhập đúng thằng chưa");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult UpdateMedicalRecord(
            [Required]
            string Id,
            [Required] IFormFile file)
        {
            try
            {

                Patient patient = FullyQueryPatientFromContext().FirstOrDefault(p => p.Id == Id);
                if(patient == null)
                {
                    return NotFound("Patient not found!");
                }

                FileUploadResult uploadResult = _servicesManager.FileUploadServices.Upload(file);

                if(uploadResult.Succeeded)
                {
                    MediaFile mediafile = patient.MedicalRecordFile;

                    if(mediafile != null)
                    {
                        mediafile.FileURL = uploadResult.FileUrl;
                    }
                    else
                    {
                        mediafile = new MediaFile() { FileURL = uploadResult.FileUrl, Category = MediaFile.FileCategory.PatientProfile };
                        patient.MedicalRecordFile = mediafile;
                    }
                    _context.Patients.Update(patient);
                    _context.SaveChanges();

                    PatientDTO patientDTO = _servicesManager.AutoMapper.Map<PatientDTO>(patient);

                    return Ok(patientDTO);
                }

                throw new Exception("File Upload Failed!");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
