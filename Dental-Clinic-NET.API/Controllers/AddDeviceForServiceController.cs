using DataLayer.DataContexts;
using DataLayer.Domain;
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
    public class AddDeviceForServiceController : ControllerBase
    {
        private AppDbContext _context;
        private ServicesManager _servicesManager;

        public AddDeviceForServiceController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
            _servicesManager = servicesManager;
        }

        [HttpPost]
        public IActionResult AddDevice(AddDevice listDevice)
        {
            try
            {
                Service service = _context.Services.Include(s => s.Devices).FirstOrDefault(s => s.Id == listDevice.Id);
                if (service == null) return BadRequest();
                service.Devices = new List<Device>();
                foreach (int id in listDevice.ListDeviceId)
                {
                    Device device = _context.Devices.Find(id);
                    if (device != null && service.Devices.FirstOrDefault(d => d.Id == id) == null)
                    {
                        service.Devices.Add(device);
                    }
                }
                _context.Services.Update(service);
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
