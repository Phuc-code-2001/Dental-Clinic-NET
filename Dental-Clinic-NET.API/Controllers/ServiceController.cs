using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using Dental_Clinic_NET.API.Models.Services;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Utils;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private ServicesManager _servicesManager;
        public ServiceController(ServicesManager servicesManager)
        {
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
                var services = _servicesManager.DbContext.Services.Include(d => d.Devices).ToArray();
                var serviceDTOs = _servicesManager.AutoMapper.Map<ServiceDTO[]>(services);

                if(page != -1)
                {
                    Paginated<ServiceDTO> paginatedServices = new Paginated<ServiceDTO>(serviceDTOs.AsQueryable(), page);
                    return Ok(new
                    {
                        page = page,
                        per_page = paginatedServices.PageSize,
                        total = paginatedServices.QueryCount,
                        total_pages = paginatedServices.PageCount,
                        data = paginatedServices.Items
                    });
                }

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
        ///     400: Invalid Info
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([FromForm] CreateService request)
        {
            try
            {

                Service service = _servicesManager.AutoMapper.Map<Service>(request);

                // Check Image File null
                if (request.ImageFile != null)
                {
                    // Check Image
                    bool imageCorrect = _servicesManager.ImageKitServices.IsImage(request.ImageFile);
                    if (!imageCorrect)
                    {
                        return BadRequest("File must be image!");
                    }
                    
                    // Upload New Image
                    var uploadImageResult = _servicesManager
                        .ImageKitServices
                        .UploadImageAsync(request.ImageFile, request.ImageFile.FileName).Result;

                    service.ImageURL = uploadImageResult.URL;
                    service.ImageId = uploadImageResult.ImageId;
                }

                // Check device inner
                service.Devices = new List<Device>();
                foreach (int id in request.DeviceIdList)
                {
                    Device device = _servicesManager.DbContext.Devices.Find(id);
                    if (device != null)
                    {
                        service.Devices.Add(device);
                    }
                }

                _servicesManager.DbContext.Services.Add(service);
                _servicesManager.DbContext.SaveChanges();

                ServiceDTO serviceDTO = _servicesManager.AutoMapper.Map<ServiceDTO>(service);

                return Ok(serviceDTO);

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
        ///     404: Not found
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Service service = _servicesManager.DbContext.Services
                    .Include(s => s.Devices).FirstOrDefault(s => s.Id == id);

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
        ///     404: Not found
        ///     500: Server handle error
        /// </returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                Service service = _servicesManager.DbContext.Services.Find(id);
                if (service == null)
                {
                    return NotFound("service not found");
                }

                _servicesManager.DbContext.Entry(service).State = EntityState.Deleted;
                _servicesManager.DbContext.SaveChanges();

                return Ok($"You just have completely delete service with id='{id}' success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Update Service Info
        /// </summary>
        /// <param name="request">New Info</param>
        /// <returns>
        ///     200: Update success
        ///     400: Invalid Info
        ///     404: Not found
        ///     500: Server handle error
        /// </returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update([FromForm] UpdateService request)
        {
            try
            {
                // Find Service
                Service service = _servicesManager.DbContext.Services
                    .Include(s => s.Devices)
                    .FirstOrDefault(s => s.Id == request.Id);

                // Check service null
                if (service == null)
                {
                    return NotFound("Service not found");
                }

                // Check Image File null
                if (request.ImageFile != null)
                {
                    // Check Image
                    bool imageCorrect = _servicesManager.ImageKitServices.IsImage(request.ImageFile);
                    if (!imageCorrect)
                    {
                        return BadRequest("File must be image!");
                    }
                    // Delete Old Image
                    if (service.ImageId != null)
                    {
                        _ = _servicesManager.ImageKitServices.DeleteImageAsync(service.ImageId);
                    }
                    // Upload New Image
                    var uploadImageResult = _servicesManager
                        .ImageKitServices
                        .UploadImageAsync(request.ImageFile, request.ImageFile.FileName).Result;

                    service.ImageURL = uploadImageResult.URL;
                    service.ImageId = uploadImageResult.ImageId;
                }
                // Map Data
                _servicesManager.AutoMapper.Map<UpdateService, Service>(request, service);

                // Setup DeviceList
                if(request.DeviceIdList != null)
                {
                    List<Device> deviceToAdd = new List<Device>();
                    List<Device> deviceToRemove = new List<Device>();
                    // Check To Add
                    foreach (int deviceId in request.DeviceIdList)
                    {
                        Device device = _servicesManager.DbContext.Devices.Find(deviceId);
                        if (device != null && !service.Devices.Contains(device))
                        {
                            deviceToAdd.Add(device);
                        }
                    }
                    // Check To Remove
                    foreach (Device device in service.Devices)
                    {
                        if (!request.DeviceIdList.Contains(device.Id))
                        {
                            deviceToRemove.Add(device);
                        }
                    }
                    // Add
                    foreach (Device device in deviceToAdd)
                    {
                        service.Devices.Add(device);
                    }
                    // Remove
                    foreach (Device device in deviceToRemove)
                    {
                        service.Devices.Remove(device);
                    }
                } 

                //Save
                _servicesManager.DbContext.Entry(service).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                // Map View
                ServiceDTO serviceDTO = _servicesManager.AutoMapper.Map<ServiceDTO>(service);

                return Ok(serviceDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
