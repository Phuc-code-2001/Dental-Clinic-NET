using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SelectBoxItemsController : ControllerBase
    {
        ServicesManager _servicesManager;

        public SelectBoxItemsController(ServicesManager servicesManager)
        {
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
                var types = _servicesManager.DbContext.Services.Select(sv => new
                {
                    Id=sv.Id,
                    Name=sv.ServiceName,
                    Code=sv.ServiceCode,
                    Description=sv.Description, // Addon to show at home
                });
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
                var types = _servicesManager.DbContext.Devices.Select(dv => new
                {
                    Id=dv.Id,
                    Name=dv.DeviceName,
                    Description=dv.Description
                });

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        
        [HttpGet]
        public IActionResult GetRooms()
        {
            try
            {
                var types = _servicesManager.DbContext.Rooms.Select(r => new
                {
                    Id=r.Id,
                    Code=r.RoomCode,
                    Description=r.Description
                });
                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public IActionResult GetAppointmentStates()
        {
            try
            {
                var types = _servicesManager.AutoMapper
                    .Map<EnumTypeDTO[]>(Enum.GetValues<Appointment.States>());

                return Ok(types);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetSlots()
        {
            try
            {
                var result = Enum.GetValues<TimeManager.SlotManager>().Select(slot => new
                {
                    id = slot,
                    name = slot.ToString(),
                    short_description = TimeManager.Instance.TryConvertToStrTime(slot),
                    details = TimeManager.Instance.GetTime(slot),
                });

                return Ok(result);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetUserRoles()
        {
            try
            {
                var roles = _servicesManager.AutoMapper.Map<EnumTypeDTO[]>(Enum.GetValues<UserType>());
                return Ok(roles);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
