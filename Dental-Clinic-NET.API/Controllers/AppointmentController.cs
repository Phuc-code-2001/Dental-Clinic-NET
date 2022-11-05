using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Appointments;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        AppDbContext _context;
        ServicesManager _servicesManager;

        public AppointmentController(AppDbContext context, ServicesManager servicesManager)
        {
            _context = context;
            _servicesManager = servicesManager;
        }

        private IQueryable<Appointment> QueryAll()
        {
            return _context.Appointments
                .Include(apt => apt.Patient)
                .Include(apt => apt.Doctor)
                .Include(apt => apt.Service)
                .Include(apt => apt.Room);
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] AppointmentFilter filter, int page = 1)
        {
            try
            {

                var queryAll = QueryAll();
                var filtered = filter.Filter(queryAll);

                Paginated<Appointment> paginated = new Paginated<Appointment>(filtered, page);

                Appointment[] items = paginated.Items.ToArray();
                AppointmentDTO[] itemDTOs = _servicesManager.AutoMapper.Map<AppointmentDTO[]>(items);

                return Ok(new
                {
                    page = page,
                    per_page = paginated.PageSize,
                    total = paginated.QueryCount,
                    total_pages = paginated.PageCount,
                    data = itemDTOs
                });

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

    }
}
