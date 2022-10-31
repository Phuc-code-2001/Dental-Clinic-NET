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
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var doctors = _context.Doctors.Include(d => d.BaseUser)
                    .Select(doc => _servicesManager.AutoMapper.Map<DoctorDTO>(doc)).ToList();
                return Ok(doctors);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPost]
        public IActionResult UpdateUserToDoctor(UpdateDoctor request)
        {
            try
            {
                BaseUser user = _context.Users.Find(request.Id);
                if (user == null)
                {
                    return NotFound("user not found");
                }
                Doctor doctor = _servicesManager.AutoMapper.Map<Doctor>(request);
                doctor.BaseUser = user;
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

                Console.WriteLine("Response done at: " + DateTime.Now);
                return Ok($"Update user to doctor success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Update(UpdateDoctor request)
        {
            try
            {
                Doctor doctor = _context.Doctors.Find(request.Id);
                if (doctor == null)
                {
                    return NotFound("Doctor not found");
                }
                _servicesManager.AutoMapper.Map<UpdateDoctor, Doctor>(request, doctor);
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

                Console.WriteLine("Response done at: " + DateTime.Now);
                return Ok($"Update doctor success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
