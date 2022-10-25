using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Devices;
using Dental_Clinic_NET.API.Models.Services;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AddServiceForDeviceController : Controller
    {
        private AppDbContext _context;
        private ServicesManager _servicesManager;

        public AddServiceForDeviceController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
            _servicesManager = servicesManager;
        }

        [HttpPost]
        public IActionResult AddService(AddService listService)
        {
            try
            {
                Device device = _context.Devices.Include(d => d.Services).FirstOrDefault(d => d.Id == listService.Id);
                if (device == null) return BadRequest();
                device.Services = new List<Service>();
                foreach (int id in listService.ListServiceId)
                {
                    Service service = _context.Services.Find(id);
                    if (service != null && device.Services.FirstOrDefault(s => s.Id == id) == null)
                    {
                        device.Services.Add(service);
                    }
                }
                _context.Devices.Update(device);
                _context.SaveChanges();
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
