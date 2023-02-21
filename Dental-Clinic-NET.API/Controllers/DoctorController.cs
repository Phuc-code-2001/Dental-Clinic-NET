using DataLayer.Domain;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using Dental_Clinic_NET.API.Models.Devices;
using Dental_Clinic_NET.API.Models.Doctors;
using DataLayer.DataContexts;
using Dental_Clinic_NET.API.Models.Users;
using System.Linq;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Utils;
using System.IO;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult GetAll(int page = 1)
        {
            try
            {
                var queries = _servicesManager.DbContext.Doctors
                    .Include(d => d.Certificate)
                    .Include(d => d.BaseUser);

                Paginated<Doctor> paginated = new Paginated<Doctor>(queries, page);
                Doctor[] doctors = paginated.Items.ToArray();
                DoctorDTO[] doctorDTOs = _servicesManager.AutoMapper.Map<DoctorDTO[]>(doctors);

                return Ok(new
                {
                    page = page,
                    per_page = paginated.PageSize,
                    total = paginated.QueryCount,
                    total_pages = paginated.PageCount,
                    data = doctorDTOs
                });

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

                // Push event
                string[] chanels = _servicesManager.DbContext.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Doctor-Update", doctor, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

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
        public IActionResult Get(string id)
        {
            try
            {
                Doctor doctor = _servicesManager.DbContext.Doctors
                    .Include(d => d.BaseUser)
                    .Include(d => d.Certificate)
                    .FirstOrDefault(d => d.Id == id);

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
