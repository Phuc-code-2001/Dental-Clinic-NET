using DataLayer.Domain;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Dental_Clinic_NET.API.Models.Doctors;
using System.Linq;
using Dental_Clinic_NET.API.Utils;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Dental_Clinic_NET.API.DTOs;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorController : Controller
    {
        private ServicesManager _servicesManager;
        public DoctorController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        /// <summary>
        ///  List all doctors
        /// </summary>
        /// <param name="page">Paginated</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server Handle Error
        /// </returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] PageFilter filter)
        {
            try
            {
                var queries = _servicesManager.DbContext.Doctors
                    .Include(d => d.Certificate)
                    .Include(d => d.BaseUser)
                    .ThenInclude(user => user.UserLocks);

                Paginated<Doctor> paginated = new Paginated<Doctor>(queries, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<DoctorDTO[]>(items.ToArray()));

                return Ok(dataset);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> CreateDoctorAsync([FromForm] CreateDoctor request)
        {
            try
            {
                BaseUser baseUser = await _servicesManager.UserManager.FindByNameAsync(request.UserName);
                if (baseUser != null)
                {
                    return BadRequest("Username already exist!");
                }
                // User Info
                baseUser = new BaseUser();
                baseUser.UserName = request.UserName;
                baseUser.FullName = request.FullName;
                baseUser.Gender = request.Gender;
                baseUser.Type = UserType.Doctor;

                // Doctor Info
                Doctor doctor = new Doctor()
                {
                    Id = baseUser.Id,
                    BaseUser = baseUser,
                    Major = request.Major,
                    Verified = true,
                };

                if(request.CertificateFile != null)
                {
                    string filename = $"certificate_{request.UserName}" + Path.GetExtension(request.CertificateFile.FileName);
                    try
                    {
                        doctor.Certificate = await _servicesManager.DoctorServices
                            .UploadCertificateAsync(request.CertificateFile, filename);
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }

                _servicesManager.DbContext.Doctors.Add(doctor);
                var createdResult = await _servicesManager.UserManager.CreateAsync(baseUser, request.Password);
                if(!createdResult.Succeeded)
                {
                    var errors = createdResult.Errors.Select(e => e.Description);
                    return BadRequest(errors);
                }

                doctor = await _servicesManager.DbContext.Doctors.Include(d => d.BaseUser)
                    .FirstOrDefaultAsync(d => d.Id == baseUser.Id);

                return Ok(new
                {
                    message = $"The verified doctor with id='{baseUser.Id}' created.",
                    data = _servicesManager.AutoMapper.Map<DoctorDTO>(doctor)
                });

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Update doctor state, info
        /// </summary>
        /// <param name="request">Doctor info</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateAsync([FromForm] UpdateDoctor request)
        {
            try
            {
                Doctor doctor = _servicesManager.DbContext.Doctors
                    .Include(d => d.BaseUser).FirstOrDefault(d => d.Id == request.Id);

                if (doctor == null)
                {
                    return NotFound("Doctor not found");
                }

                _servicesManager.AutoMapper.Map<UpdateDoctor, Doctor>(request, doctor);

                if (request.CertificateFile != null)
                {
                    string filename = $"doctor_{doctor.BaseUser.UserName}" + Path.GetExtension(request.CertificateFile.FileName);
                    try
                    {
                        doctor.Certificate = await _servicesManager.DoctorServices
                            .UploadCertificateAsync(request.CertificateFile, filename);
                    }
                    catch(Exception ex)
                    {
                        return BadRequest(ex.Message);
                    }
                }

                _servicesManager.DbContext.Entry(doctor).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                doctor = _servicesManager.DbContext.Doctors
                    .Include(d => d.BaseUser)
                    .Include(d => d.Certificate)
                    .FirstOrDefault(d => d.Id == doctor.Id);

                DoctorDTO doctorDTO = _servicesManager.AutoMapper.Map<DoctorDTO>(doctor);

                return Ok(doctorDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Get specific doctor info
        /// </summary>
        /// <param name="id">id of doctor</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(string id)
        {
            try
            {

                Doctor doctor = await _servicesManager.DbContext.Doctors
                    .Include(d => d.Certificate)
                    .Include(d => d.BaseUser)
                    .ThenInclude(user => user.UserLocks)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if(doctor == null)
                {
                    return NotFound("Doctor not found!");
                }

                DoctorDTO doctorDTO = _servicesManager.AutoMapper.Map<DoctorDTO>(doctor);

                return Ok(doctorDTO);


            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
