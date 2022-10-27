using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Services;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using Dental_Clinic_NET.API.Models.Devices;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private AppDbContext _context;
        private ServicesManager _servicesManager;
        public DeviceController(AppDbContext context, ServicesManager servicesManager)
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
        public IActionResult GetAll(int page = 1)
        {
            try
            {
                var devices = _context.Devices.Include(d => d.Services).ToList();
                var deviceDTOs = devices.Select(device => _servicesManager.AutoMapper.Map<DeviceDTO>(device));

                Paginated<DeviceDTO> paginatedDevices = new Paginated<DeviceDTO>(deviceDTOs.AsQueryable(), page);


                return Ok(new
                {
                    page = page,
                    per_page = paginatedDevices.PageSize,
                    total = paginatedDevices.ColectionCount,
                    total_pages = paginatedDevices.PageCount,
                    data = paginatedDevices.Items
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Create new device from any actor
        /// </summary>
        /// <param name="request">Devices Info</param>
        /// <returns>
        ///     200: Create success
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        public IActionResult Create(CreateDevice request)
        {
            try
            {
                Device device = _servicesManager.AutoMapper.Map<Device>(request);
                _context.Devices.Add(device);
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Device-Create", device, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                Console.WriteLine("Response done at: " + DateTime.Now);

                return Ok(device);

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
                Device device = _context.Devices.Find(id);
                if (device == null) return NotFound("Service not found.");

                DeviceDTO deviceDTO = _servicesManager.AutoMapper.Map<DeviceDTO>(device);

                return Ok(deviceDTO);
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
                Device device = _context.Devices.Find(id);
                if (device == null)
                {
                    return NotFound("Device not found");
                }

                _context.Entry(device).State = EntityState.Deleted;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Device-Delete", new { Id = device.Id }, result =>
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
        public IActionResult Update(UpdateDevice request)
        {
            try
            {
                Device device = _context.Devices.Find(request.Id);
                if (device == null)
                {
                    return NotFound("Service not found");
                }
                if (request.DeviceName != null && request.DeviceName != "") device.DeviceName = device.DeviceName;
                if (request.Description != null && request.Description != "") device.Description = request.Description;

                _context.Entry(device).State = EntityState.Modified;
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Device-Update", device, result =>
                    {
                        Console.WriteLine("Push event done at: " + DateTime.Now);
                    });

                Console.WriteLine("Response done at: " + DateTime.Now);
                return Ok($"Update device success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
