using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SelectBoxItemsController : ControllerBase
    {
        AppDbContext _context;
        ServicesManager _servicesManager;

        public SelectBoxItemsController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
            _servicesManager = servicesManager;
        }

        /// <summary>
        /// Get all RoomType to create Room
        /// </summary>
        /// <returns>
        ///     200: Success
        ///     500: Server handle error
        /// </returns>
        [HttpGet]
        public IActionResult GetRoomTypes()
        {
            try
            {
                var types = _servicesManager.AutoMapper
                    .Map<EnumTypeDTO[]>(Enum.GetValues<Room.RoomTypes>());

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetServices()
        {
            try
            {
                var types = _context.Services.Select(sv => _servicesManager.AutoMapper.Map<EnumTypeDTO>(sv));
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet]
        public IActionResult GetDevices()
        {
            try
            {
                var types = _context.Devices.Select(dv => _servicesManager.AutoMapper.Map<EnumTypeDTO>(dv));
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
