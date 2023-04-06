using DataLayer.Domain;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Dental_Clinic_NET.API.Models.Devices;
using Dental_Clinic_NET.API.Utils;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Dental_Clinic_NET.API.DTOs;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private ServicesManager _servicesManager;
        public DeviceController(ServicesManager servicesManager)
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
        public IActionResult GetAll([FromQuery] SearchFilter<Device> filter)
        {
            try
            {
                IQueryable<Device> devices = _servicesManager.DbContext.Devices
                    .Include(d => d.Services)
                    .Include(d => d.Room);

                devices = filter.FilteredQuery(devices, (src, key) =>
                {
                    return src.Where(x => 
                        x.Id.ToString() == key || 
                        x.DeviceName.Contains(key) || 
                        x.DeviceValue.ToString().Contains(key) ||
                        x.Room.RoomCode.Contains(key)
                    );
                });

                Paginated<Device> paginated = new Paginated<Device>(devices, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<DeviceDTO[]>(items.ToArray()));

                return Ok(dataset);
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
        [Authorize(Roles = "Administrator")]
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
                device.Services = _servicesManager.DbContext.Services
                                .Where(x => request.ServiceIdList.Contains(x.Id)).ToList();

                _servicesManager.DbContext.Devices.Add(device);
                _servicesManager.DbContext.SaveChanges();

                // Include services and room
                device = _servicesManager.DbContext.Devices
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
                Device device = _servicesManager.DbContext.Devices
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
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                Device device = _servicesManager.DbContext.Devices.Find(id);
                if (device == null)
                {
                    return NotFound("Device not found");
                }

                _servicesManager.DbContext.Entry(device).State = EntityState.Deleted;
                _servicesManager.DbContext.SaveChanges();

                return Ok($"You just have completely delete service with id='{id}' success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        /// <summary>
        /// Update the device information
        /// </summary>
        /// <param name="request">Update expected data</param>
        /// <returns>
        ///     200: DeviceDTO
        ///     404: string
        ///     400: string
        ///     500: string
        /// </returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update([FromForm] UpdateDevice request)
        {
            try
            {
                Device device = _servicesManager.DbContext.Devices
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

                device.Services = _servicesManager.DbContext.Services.Where(x => request.ServiceIdList.Contains(x.Id)).ToList();

                _servicesManager.DbContext.Entry(device).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

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
