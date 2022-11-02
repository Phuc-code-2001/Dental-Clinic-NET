﻿using DataLayer.DataContexts;
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
using Dental_Clinic_NET.API.Models.Users;
using System.Collections.Generic;

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
                var devices = _context.Devices
                    .Include(d => d.Services)
                    .Include(d => d.Room).ToArray();
                var deviceDTOs = _servicesManager.AutoMapper.Map<DeviceDTO[]>(devices);

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
        ///     400: Image failed, or error request handle
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        public IActionResult Create([FromForm] CreateDevice request)
        {
            try
            {

                Device device = _servicesManager.AutoMapper.Map<Device>(request);

                if (request.ImageFile != null)
                {
                    // Check Image
                    bool imageCorrect = _servicesManager.ImageKitServices.IsImage(request.ImageFile);
                    if(!imageCorrect)
                    {
                        return BadRequest("File must be image!");
                    }

                    var uploadImageResult = _servicesManager
                        .ImageKitServices
                        .UploadImageAsync(request.ImageFile, request.ImageFile.FileName).Result;

                    device.ImageURL = uploadImageResult.URL;
                    device.ImageId = uploadImageResult.ImageId;
                }


                // Check service inner
                device.Services = new List<Service>();
                foreach(int id in request.ServiceIdList)
                {
                    Service service = _context.Services.Find(id);
                    if(service != null)
                    {
                        device.Services.Add(service);
                    }
                }

                _context.Devices.Add(device);
                _context.SaveChanges();

                // Push event
                string[] chanels = _context.Users.Where(user => user.Type == UserType.Administrator)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Device-Create", device, result =>
                    {
                        
                    });

                // Include services and room
                device = _context.Devices
                    .Include(d => d.Services)
                    .Include(d => d.Room)
                    .FirstOrDefault(d => d.Id == device.Id);

                DeviceDTO deviceDTO = _servicesManager.AutoMapper.Map<DeviceDTO>(device);
                return Ok(deviceDTO);

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
                // Include services and room
                Device device = _context.Devices
                    .Include(d => d.Services)
                    .Include(d => d.Room)
                    .FirstOrDefault(d => d.Id == id);

                if (device == null) return NotFound("Device not found.");

                DeviceDTO deviceDTO = _servicesManager.AutoMapper.Map<DeviceDTO>(device);

                return Ok(deviceDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Remove device out of database
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

                return Ok($"You just have completely delete service with id='{id}' success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpPut]
        public IActionResult Update([FromForm] UpdateDevice request)
        {
            try
            {
                Device device = _context.Devices
                    .Include(d => d.Services)
                    .Include(d => d.Room)
                    .FirstOrDefault(d => d.Id == request.Id);
                
                if (device == null)
                {
                    return NotFound("Device not found");
                }

                if(request.ImageFile != null)
                {
                    // Check Image
                    bool imageCorrect = _servicesManager.ImageKitServices.IsImage(request.ImageFile);
                    if (!imageCorrect)
                    {
                        return BadRequest("File must be image!");
                    }
                    
                    if(device.ImageId != null)
                    {
                        _ = _servicesManager.ImageKitServices.DeleteImageAsync(device.ImageId);
                    }

                    var uploadImageResult = _servicesManager
                        .ImageKitServices
                        .UploadImageAsync(request.ImageFile, request.ImageFile.FileName).Result;

                    device.ImageURL = uploadImageResult.URL;
                    device.ImageId = uploadImageResult.ImageId;
                    
                }

                _servicesManager.AutoMapper.Map<UpdateDevice, Device>(request, device);


                if(request.ServiceIdList != null)
                {
                    List<Service> serviceToAdd = new List<Service>();
                    List<Service> serviceToRemove = new List<Service>();

                    foreach(int serviceId in request.ServiceIdList)
                    {
                        Service service = _context.Services.Find(serviceId);
                        if(!device.Services.Contains(service)) {
                            serviceToAdd.Add(service);
                        }
                    }

                    foreach(Service service in device.Services)
                    {
                        if(!request.ServiceIdList.Contains(service.Id))
                        {
                            serviceToRemove.Add(service);
                        }
                    }

                    foreach(Service service in serviceToAdd)
                    {
                        device.Services.Add(service);
                    }
                    foreach(Service service in serviceToRemove)
                    {
                        device.Services.Remove(service);
                    }
                }

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

                DeviceDTO deviceDTO = _servicesManager.AutoMapper.Map<DeviceDTO>(device);

                return Ok(deviceDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
