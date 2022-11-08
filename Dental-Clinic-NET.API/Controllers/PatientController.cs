using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Patients;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
            return _context.Patients
                .Include(pat => pat.BaseUser)
                .Include(pat => pat.MedicalRecordFile)
                .Where(pat => pat.BaseUser.Type == UserType.Patient);
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
        public IActionResult GetAll(int page = 1)
        {
            try
            {
                var queries = FullyQueryPatientFromContext();
                var paginated = new Paginated<Patient>(queries, page);

                var patientDTOs = paginated.Items.Select(pat => _servicesManager.AutoMapper.Map<PatientDTO>(pat));

                return Ok(new
                {
                    page = page,
                    per_page = paginated.PageSize,
                    total = paginated.QueryCount,
                    total_pages = paginated.PageCount,
                    data = patientDTOs
                });

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

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
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

        /// <summary>
        /// Update MedicalRecord for Patient
        /// </summary>
        /// <param name="request">request info</param>
        /// <returns>
        ///     200: Request success
        ///     403: Forbiden
        ///     404: Patient not found
        ///     400: Some field invalid
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateMedicalRecordAsync([FromForm] UpdateMedicalRecordModel request)
        {
            try
            {

                Patient patient = FullyQueryPatientFromContext().FirstOrDefault(p => p.Id == request.Id);

                if (patient.BaseUser.Type != UserType.Patient)
                {
                    return BadRequest("This user is not patient!");
                }

                if (patient == null)
                {
                    return NotFound("Patient not found!");
                }

                BaseUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, patient.BaseUser);

                if (!permission.IsOwner && !permission.IsAdmin)
                {
                    return StatusCode(403, "Only admin or Owner can update medical record!");
                }

                if (!request.File.ContentType.EndsWith("pdf"))
                {
                    return BadRequest("Only Accept PDF file!");
                }

                string filename = $"patient_{request.Id}_" + Path.GetExtension(request.File.FileName);
                var uploadResult = await _servicesManager.DropboxServices.UploadAsync(request.File, filename);

                MediaFile mediafile = patient.MedicalRecordFile;

                if(mediafile != null)
                {
                    mediafile.FilePath = uploadResult.UploadPath;
                }
                else
                {
                    mediafile = new MediaFile() 
                    { 
                        FilePath = uploadResult.UploadPath,
                        Category = MediaFile.FileCategory.MedicalRecord
                    };

                    patient.MedicalRecordFile = mediafile;
                }

                _context.Patients.Update(patient);
                _context.SaveChanges();

                PatientDTO patientDTO = _servicesManager.AutoMapper.Map<PatientDTO>(patient);

                return Ok(patientDTO);
               
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
