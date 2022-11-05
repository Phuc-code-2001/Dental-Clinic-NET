using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Appointments;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Services.Appointments;
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
    public class AppointmentController : ControllerBase
    {
        AppDbContext _context;
        ServicesManager _servicesManager;
        AppointmentServices _appointmentServices;

        public AppointmentController(AppDbContext context, ServicesManager servicesManager, AppointmentServices appointmentServices)
        {
            _context = context;
            _servicesManager = servicesManager;
            _appointmentServices = appointmentServices;
        }

        private IQueryable<Appointment> QueryAll()
        {
            return _context.Appointments
                .Include(apt => apt.Patient)
                .Include(apt => apt.Doctor)
                .Include(apt => apt.Service)
                .Include(apt => apt.Documents)
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] CreateAppointment request)
        {

            try
            {
                Appointment entity = _servicesManager.AutoMapper.Map<Appointment>(request);
                if(request.Document != null)
                {
                    // < Xử lý file >
                    if(!request.Document.ContentType.EndsWith("docx"))
                    {
                        return BadRequest("Document file must be *.docx format!");
                    }

                    AppointmentDocument document = new AppointmentDocument();
                    document.Title = "Medical Record Profile for Appontment";
                    document.Document = new MediaFile()
                    {
                        Category = MediaFile.FileCategory.AppointmentDocument
                    };

                    string filename = $"apm_{request.PatientId}+{DateTime.Now.Ticks}.docx";
                    var result = await _servicesManager.DropboxServices.UploadAsync(request.Document, filename);

                    document.Document.FilePath = result.UploadPath;
                    
                    entity.Documents.Add(document);

                    // </ Xử lý file >
                }

                entity.Doctor = _appointmentServices.FindDoctorForAppointment(entity);
                entity.Room = _appointmentServices.FindRoomForAppointment(entity);

                _context.Appointments.Add(entity);
                _context.SaveChanges();

                entity = QueryAll().FirstOrDefault(e => e.Id == entity.Id);

                return Ok(entity);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            
        }

    }
}
