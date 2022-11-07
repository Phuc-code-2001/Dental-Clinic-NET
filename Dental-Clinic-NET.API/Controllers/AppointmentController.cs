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
                .Include(apt => apt.Room.Devices)
                .Include(apt => apt.Documents)
                .ThenInclude(d => d.Document);
        }

        private static bool CanRead(Appointment entity, BaseUser user)
        {
            var permission = new PermissionOnAppointment(user, entity);
            bool c1 = permission.IsAdmin;
            bool c2 = permission.IsOwner;
            bool c3 = permission.LoggedUser.Type == UserType.Receptionist;
            return c1 || c2 || c3;
        }

        private static bool CanWrite(Appointment entity, BaseUser user)
        {
            var permission = new PermissionOnAppointment(user, entity);

            switch(user.Type)
            {
                case UserType.Patient:
                    // Only allowed in upload document
                    return permission.IsOwner;

                case UserType.Doctor:
                    return permission.IsOwner 
                        && entity.State == Appointment.States.Accept
                        && entity.State == Appointment.States.Doing;

                case UserType.Receptionist:
                    return entity.State == Appointment.States.NotYet;

                case UserType.Administrator:
                    return true;
            }

            return false;

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
                    .Where(entity => CanRead(entity, loggedUser)).AsQueryable();

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

        /// <summary>
        ///     Create new Appointment, by admin or by patient
        /// </summary>
        /// <param name="request">Appointment information</param>
        /// <returns>
        ///     200: Create success
        ///     401: Unauthorize
        ///     403: Forbiden
        ///     400: Some field invalid
        ///     500: Server handle error
        /// </returns>
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
                    document.Tag = AppointmentDocument.DocumentTags.Patient;
                    document.Document = new MediaFile()
                    {
                        Category = MediaFile.FileCategory.AppointmentDocument
                    };

                    string filename = $"apm_{entity.Id}_Patient+{DateTime.Now.Ticks}" + Path.GetExtension(request.Document.FileName);
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

        /// <summary>
        ///     Get details of an appointment
        /// </summary>
        /// <param name="id">id of appointment</param>
        /// <returns>
        ///     200: Request success
        ///     404: Not found
        ///     403: Forbiden
        ///     401: Unauthorize
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult Get(int id)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

                if(!CanRead(entity, loggedUser))
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

        /// <summary>
        ///  Transform an appointment to 'Accept' state
        /// </summary>
        /// <param name="id">id of appointment</param>
        /// <returns>
        ///     200: Request success
        ///     404: Not found
        ///     403: Forbiden
        ///     401: Unauthorize
        ///     500: Server handle error
        /// </returns>
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

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if(!CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
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

        /// <summary>
        ///  Transform an appointment to 'Cancel' state
        /// </summary>
        /// <param name="id">id of appointment</param>
        /// <returns>
        ///     200: Request success
        ///     404: Not found
        ///     403: Forbiden
        ///     401: Unauthorize
        ///     500: Server handle error
        /// </returns>
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

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (!CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
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

        /// <summary>
        ///  Transform an appointment to 'Doing' state
        /// </summary>
        /// <param name="id">id of appointment</param>
        /// <returns>
        ///     200: Request success
        ///     404: Not found
        ///     403: Forbiden
        ///     401: Unauthorize
        ///     500: Server handle error
        /// </returns>
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Doctor) + "," + nameof(UserType.Administrator))]
        public IActionResult Doing(int id)
        {
            try
            {
                Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

                if (entity == null)
                {
                    return NotFound("Truyền sai id rồi => Appointment not found!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (!CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
                }

                entity.State = Appointment.States.Doing;
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

        /// <summary>
        ///  Transform an appointment to 'Complete' state
        /// </summary>
        /// <param name="id">id of appointment</param>
        /// <returns>
        ///     200: Request success
        ///     404: Not found
        ///     403: Forbiden
        ///     401: Unauthorize
        ///     500: Server handle error
        /// </returns>
        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Doctor) + "," + nameof(UserType.Administrator))]
        public IActionResult Complete(int id)
        {
            try
            {
                Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

                if (entity == null)
                {
                    return NotFound("Truyền sai id rồi => Appointment not found!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (!CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
                }

                entity.State = Appointment.States.Complete;
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

        /// <summary>
        ///     Allow doctors adding a document for their appointment
        /// </summary>
        /// <param name="requestModel">Contain document information</param>
        /// <returns>
        ///     200: Request success
        ///     404: Not found
        ///     403: Forbiden
        ///     401: Unauthorize
        ///     400: Some fields invalid
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = nameof(UserType.Doctor) + "," + nameof(UserType.Administrator))]
        public async Task<IActionResult> DoctorAddDocumentAsync([FromForm] AddDocumentModel requestModel)
        {
            try
            {
                Appointment entity = QueryAll().FirstOrDefault(item => item.Id == requestModel.AppointmentId);

                if(entity == null)
                {
                    return NotFound("Appointment not found! Kiểm tra lại 'appoinmentId'!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (!CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
                }

                AppointmentDocument document = _servicesManager.AutoMapper.Map<AppointmentDocument>(requestModel);
                if(string.IsNullOrWhiteSpace(document.Title))
                {
                    document.Title = "Doctor document for appointment";
                }

                document.Tag = AppointmentDocument.DocumentTags.Doctor;

                IFormFile file = requestModel.DocumentFile;

                // < Xử lý file >
                if (!file.ContentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                {
                    return BadRequest("Document file must be *.docx format!");
                }

                document.Document = new MediaFile()
                {
                    Category = MediaFile.FileCategory.AppointmentDocument
                };

                string filename = $"apm_{entity.Id}_Doctor+{DateTime.Now.Ticks}" + Path.GetExtension(file.FileName);
                var result = await _servicesManager.DropboxServices.UploadAsync(file, filename);

                document.Document.FilePath = result.UploadPath;

                entity.Documents.Add(document);
                _context.Entry(entity).State = EntityState.Modified;
                _context.SaveChanges();
                // </ Xử lý file >

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
