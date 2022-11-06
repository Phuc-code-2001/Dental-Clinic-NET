using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Appointments;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Services.Appointments;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
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
                .Include(apt => apt.Patient.BaseUser)
                .Include(apt => apt.Patient.MedicalRecordFile)
                .Include(apt => apt.Doctor.BaseUser)
                .Include(apt => apt.Doctor.Certificate)
                .Include(apt => apt.Service.Devices)
                .Include(apt => apt.Documents)
                .Include(apt => apt.Room.Devices);
        }

        private bool CanGet(Appointment entity, BaseUser user)
        {
            var permission = new PermissionOnAppointment(user, entity);
            bool c1 = permission.IsAdmin;
            bool c2 = permission.IsOwner;
            bool c3 = permission.LoggedUser.Type == UserType.Receptionist;
            return c1 || c2 || c3;
        }

        /// <summary>
        ///     Retrive a list of appointments, all which can view by logged user.
        /// </summary>
        /// <param name="filter">Filter properties</param>
        /// <param name="page">Pageing</param>
        /// <returns>
        ///     200: Query success, return list of appointments
        ///     500: Server handle error
        /// </returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetAll([FromQuery] AppointmentFilter filter, int page = 1)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                var queryAll = QueryAll();
                var filtered = filter.Filter(queryAll);
                var permissionFiltered = filtered.AsEnumerable()
                    .Where(entity => CanGet(entity, loggedUser)).AsQueryable();

                if(page != -1)
                {
                    Paginated<Appointment> paginated = new Paginated<Appointment>(permissionFiltered, page);

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
                else
                {
                    AppointmentDTO[] itemDTOs = _servicesManager.AutoMapper.Map<AppointmentDTO[]>(permissionFiltered.ToArray());
                    return Ok(itemDTOs);

                }

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
            
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Patient) + "," + nameof(UserType.Administrator))]
        public async Task<IActionResult> CreateAsync([FromForm] CreateAppointment request)
        {

            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                Appointment entity = _servicesManager.AutoMapper.Map<Appointment>(request);

                var permission = new PermissionOnAppointment(loggedUser, entity);

                if(!permission.IsOwner && !permission.IsAdmin)
                {
                    return StatusCode(403, "Hành động bị chặn do sai quyền!");
                }

                // Check Service
                if(_context.Services.Find(request.ServiceId) == null)
                {
                    return BadRequest("Truyền sai serviceId rồi => Service not found");
                }

                if(request.Document != null)
                {
                    // < Xử lý file >
                    if(!request.Document.ContentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                    {
                        return BadRequest("Document file must be *.docx format!");
                    }

                    AppointmentDocument document = new AppointmentDocument();
                    document.Title = "Medical Record Profile for Appontment";
                    document.Document = new MediaFile()
                    {
                        Category = MediaFile.FileCategory.AppointmentDocument
                    };

                    string filename = $"apm_{request.PatientId}+{DateTime.Now.Ticks}" + Path.GetExtension(request.Document.FileName);
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
                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

            
        }

        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

                if(!CanGet(entity, loggedUser))
                {
                    return StatusCode(403, "Quyền đâu mà xem!");
                }

                if(entity == null)
                {
                    return NotFound("Truyền sai id rồi => Appointment not found!");
                }

                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Receptionist) + "," + nameof(UserType.Administrator))]
        public IActionResult Accept(int id)
        {
            try
            {
                Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

                if (entity == null)
                {
                    return NotFound("Truyền sai id rồi => Appointment not found!");
                }

                entity.State = Appointment.States.Accept;
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();

                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Receptionist) + "," + nameof(UserType.Administrator))]
        public IActionResult Cancel(int id)
        {
            try
            {
                Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

                if (entity == null)
                {
                    return NotFound("Truyền sai id rồi => Appointment not found!");
                }

                entity.State = Appointment.States.Cancel;
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();

                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
