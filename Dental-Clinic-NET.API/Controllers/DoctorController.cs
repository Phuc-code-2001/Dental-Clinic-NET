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

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DoctorController : Controller
    {
        private AppDbContext _context;
        private ServicesManager _servicesManager;
        public DoctorController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
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

                Paginated<Doctor> paginated = new Paginated<Doctor>(_context.Doctors.Include(d => d.BaseUser), page);

                Doctor[] doctors = paginated.Items.ToArray();

                DoctorDTO[] doctorDTOs = _servicesManager.AutoMapper.Map<DoctorDTO[]>(doctors);
                return Ok(doctorDTOs);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///  Make an account become a doctor
        /// </summary>
        /// <param name="request">Doctor info</param>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        public IActionResult RequestToBecomeDoctor(RequestDoctor request)
        {
            try
            {
                BaseUser user = _context.Users.Find(request.Id);
                if (user == null)
                {
                    return NotFound("User not found!");
                }

                Doctor doctor = _context.Doctors.Find(request.Id);

                if (doctor != null)
                {
                    return BadRequest("This user already doctor role!");
                }

                doctor = _servicesManager.AutoMapper.Map<Doctor>(request);
                
                if(doctor.Verified)
                {
                    user.Type = UserType.Doctor;
                }
                
                // < Xử lý file


                // Xử lý file />

                _context.Doctors.Add(doctor);

                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "User to Doctor", doctor, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                doctor = _context.Doctors
                    .Include(d => d.BaseUser)
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
        /// Update doctor state, info
        /// </summary>
        /// <param name="request">Doctor info</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpPut]
        public IActionResult Update(RequestDoctor request)
        {
            try
            {
                Doctor doctor = _context.Doctors.Find(request.Id);

                if (doctor == null)
                {
                    return NotFound("Doctor not found");
                }

                _servicesManager.AutoMapper.Map<RequestDoctor, Doctor>(request, doctor);

                BaseUser user = _context.Users.Find(doctor.Id);
                if(!doctor.Verified)
                {
                    if(user != null && user.Type == UserType.Doctor)
                    {
                        user.Type = UserType.Patient;
                        _context.Entry(user).State = EntityState.Modified;
                    }
                }
                else
                {
                    if(user != null && user.Type != UserType.Doctor)
                    {
                        user.Type = UserType.Doctor;
                        _context.Entry(user).State = EntityState.Modified;
                    }
                }

                // < Xử lý file


                // Xử lý file />

                _context.Entry(doctor).State = EntityState.Modified;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Doctor-Update", doctor, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                return Ok($"Update doctor success");
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
                Doctor doctor = _context.Doctors
                    .Include(d => d.BaseUser)
                    .FirstOrDefault(d => d.Id == id);

                if(doctor == null)
                {
                    return NotFound("Doctor not found!");
                }

                DoctorDTO doctorDTO = _servicesManager.AutoMapper.Map<DoctorDTO>(doctor);

                return Ok(doctor);


            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
    }
}
