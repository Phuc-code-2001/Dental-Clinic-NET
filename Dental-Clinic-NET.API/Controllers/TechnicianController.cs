using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Technician;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TechnicianController : ControllerBase
    {
        ServicesManager _servicesManager;

        public TechnicianController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }


        [HttpGet]
        public IActionResult GetAppointmentQueue([FromQuery] PageFilter filter)
        {
            try
            {
                var queries = _servicesManager.DbContext.Appointments
                    .Where(x => x.State == Appointment.States.Transfer 
                    || x.State == Appointment.States.TransferDoing
                    || x.State == Appointment.States.TransferCancel
                    || x.State == Appointment.States.TransferComplete)
                    .OrderBy(x => x.State);

                Paginated<Appointment> paginated = new Paginated<Appointment>(queries, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items =>
                {
                    return _servicesManager.AutoMapper.Map<AppointmentDTO[]>(items.ToArray());
                });

                return Ok(dataset);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> UploadXRayImageAsync(UploadXRayForm form)
        {
            try
            {
                // Find Appointment
                Appointment appointment = await _servicesManager.DbContext.Appointments
                                .FirstOrDefaultAsync(x => x.Id == form.AppointmentId);

                if(appointment == null)
                {
                    return NotFound("Appointment not found!");
                }

                // Do some thing

                return Ok();

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


    }
}
