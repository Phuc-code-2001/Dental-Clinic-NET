using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Room;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using Dental_Clinic_NET.API.Models.Services;
using Dental_Clinic_NET.API.DTO;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : Controller
    {
        private AppDbContext _context;
        private ServicesManager _servicesManager;
        public ServiceController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
            _servicesManager = servicesManager;
        }
        /// <summary>
        ///     List all services by admin
        /// </summary>
        /// <returns>
        ///     200: Request success
        ///     500: Server Handle Error
        ///     
        /// </returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var services = _context.Services.Include(d => d.Devices).ToList();

                var serviceDTOs = services.Select(services => _servicesManager.AutoMapper.Map<ServiceDTO>(services));

                return Ok(serviceDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Create new services from any actor
        /// </summary>
        /// <param name="request">Services Info</param>
        /// <returns>
        ///     200: Create success
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        public IActionResult Create(CreateService request)
        {
            try
            {
                Service service = _servicesManager.AutoMapper.Map<Service>(request);
                _context.Services.Add(service);
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Service-Create", service, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                Console.WriteLine("Response done at: " + DateTime.Now);

                return Ok(service);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Get a Service details
        /// </summary>
        /// <param name="id">service id</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Service service = _context.Services.Find(id);
                if (service == null) return NotFound("Service not found.");

                ServiceDTO serviceDTO = _servicesManager.AutoMapper.Map<ServiceDTO>(service);

                return Ok(serviceDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Remove service out of database
        /// </summary>
        /// <param name="id">service id</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Service service = _context.Services.Find(id);
                if (service == null)
                {
                    return NotFound("service not found");
                }

                _context.Entry(service).State = EntityState.Deleted;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Service-Delete", new { Id = service.Id }, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                Console.WriteLine("Response done at: " + DateTime.Now);

                return Ok($"You just have completely delete service with id='{id}' success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Update(UpdateService request)
        {
            try
            {
                Service service = _context.Services.Find(request.Id);
                if (service == null)
                {
                    return NotFound("Service not found");
                }
                if (request.ServiceCode != null && request.ServiceCode != "") service.ServiceCode = request.ServiceCode;
                if (request.Description != null && request.Description != "") service.Description = request.Description;
                service.price = request.price;
                _context.Entry(service).State = EntityState.Modified;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Service-Update", service, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                Console.WriteLine("Response done at: " + DateTime.Now);
                return Ok($"Update service success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
