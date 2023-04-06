using DataLayer.Domain;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Dental_Clinic_NET.API.Models.Services;
using Dental_Clinic_NET.API.Utils;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Dental_Clinic_NET.API.DTOs;
using System.Threading.Tasks;

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
        ///     List all queries by admin
        /// </summary>
        /// <returns>
        ///     200: Request success
        ///     500: Server Handle Error
        /// </returns>
        [HttpGet]
        public IActionResult GetAll([FromQuery] ServicesFilter filter)
        {
            try
            {
                IQueryable<Service> queries = _servicesManager.DbContext.Services
                    .Include(s => s.Devices);

                bool onlyShowPublic = true;
                if(User.Identity.IsAuthenticated)
                {
                    BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                    if(loggedUser.Type == UserType.Administrator)
                    {
                        onlyShowPublic = false;
                    }
                }

                if (onlyShowPublic) queries = queries.Where(service => service.IsPublic);

                var filtered = filter.GetFilteredQuery(queries);
                var paginated = new Paginated<Service>(filtered, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<ServiceDTOLite[]>(items.ToArray()));
                return Ok(dataset);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        /// <summary>
        ///     Create new queries from any actor
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
                service.Devices = _servicesManager.DbContext.Devices
                    .Where(x => request.DeviceIdList.Contains(x.Id)).ToList();

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
                service.Devices = _servicesManager.DbContext.Devices
                    .Where(x => request.DeviceIdList.Contains(x.Id)).ToList();

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


        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> MakePublicAsync(int id)
        {
            try
            {
                Service service = await _servicesManager.DbContext.Services.FirstOrDefaultAsync(x => x.Id == id);
                if(service == null)
                {
                    return NotFound($"Service with id={id} not found!");
                }

                service.IsPublic = true;
                _servicesManager.DbContext.Services.Update(service);
                _servicesManager.DbContext.SaveChanges();

                return Ok(service);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> MakeHiddenAsync(int id)
        {
            try
            {
                Service service = await _servicesManager.DbContext.Services.FirstOrDefaultAsync(x => x.Id == id);
                if(service == null)
                {
                    return NotFound($"Service with id={id} not found!");
                }

                service.IsPublic = false;
                _servicesManager.DbContext.Services.Update(service);
                _servicesManager.DbContext.SaveChanges();

                return Ok(service);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
