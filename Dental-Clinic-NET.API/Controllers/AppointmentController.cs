using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Appointments;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        ServicesManager _servicesManager;

        public AppointmentController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        private IQueryable<Appointment> QueryAll()
        {
            return _servicesManager.DbContext.Appointments
                .Include(apt => apt.Patient.BaseUser)
                .Include(apt => apt.Patient.MedicalRecordFile)
                .Include(apt => apt.Doctor.BaseUser)
                .Include(apt => apt.Doctor.Certificate)
                .Include(apt => apt.Service.Devices)
                .Include(apt => apt.Room.Devices)
                .Include(apt => apt.Documents)
                .ThenInclude(d => d.File);
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
        public IActionResult GetAll([FromQuery] AppointmentFilter filter)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                var queryAll = QueryAll();
                var filtered = filter.Filter(queryAll);
                var permissionFiltered = filtered.AsEnumerable()
                    .Where(entity => _servicesManager.AppointmentServices.CanRead(entity, loggedUser)).AsQueryable();

                Paginated<Appointment> paginated = new Paginated<Appointment>(permissionFiltered, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<AppointmentDTOLite[]>(items.ToArray()));
                return Ok(dataset);

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
        [Authorize(Roles = nameof(UserType.Patient) + "," + nameof(UserType.Administrator) + "," + nameof(UserType.Receptionist))]
        public async Task<IActionResult> CreateAsync([FromForm] CreateAppointment request)
        {

            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                Appointment entity = _servicesManager.AutoMapper.Map<Appointment>(request);

                var permission = new PermissionOnAppointment(loggedUser, entity);

                if(!permission.IsOwner && !permission.IsAdmin && loggedUser.Type != UserType.Receptionist)
                {
                    return StatusCode(403, "Hành động bị chặn do sai quyền!");
                }

                // Check Service
                if(_servicesManager.DbContext.Services.Find(request.ServiceId) == null)
                {
                    return BadRequest("Truyền sai serviceId rồi => Service not found");
                }

                if(request.Document != null)
                {
                    // < Xử lý file >
                    if(!request.Document.ContentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                    {
                        return BadRequest("File file must be *.docx format!");
                    }

                    Document document = new Document();
                    document.Title = "Medical Record Profile for Appontment";
                    document.Tag = Document.DocumentTags.Patient;
                    document.File = new FileMedia()
                    {
                        Category = FileMedia.FileCategory.AppointmentDocument
                    };

                    string filename = $"apm_{entity.Id}_Patient+{DateTime.Now.Ticks}" + Path.GetExtension(request.Document.FileName);
                    var result = await _servicesManager.DropboxServices.UploadAsync(request.Document, filename);

                    document.File.FilePath = result.UploadPath;
                    
                    entity.Documents.Add(document);

                    // </ Xử lý file >
                }

                entity.Doctor = _servicesManager.AppointmentServices.FindDoctorForAppointment(entity);
                entity.Room = _servicesManager.AppointmentServices.FindRoomForAppointment(entity);

                _servicesManager.DbContext.Appointments.Add(entity);
                _servicesManager.DbContext.SaveChanges();

                entity = QueryAll().FirstOrDefault(e => e.Id == entity.Id);
                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                // Create Notification
                Notification notification = new Notification()
                {
                    Receiver = entity.Patient.BaseUser,
                    Content = "Your appointment was created",
                    Category = Notification.NotificationCategories.Success,
                };
                _servicesManager.DbContext.Notifications.Add(notification);
                _servicesManager.DbContext.SaveChanges();
                _servicesManager.NotificationServices.SendToClient(notification);

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

                if(!_servicesManager.AppointmentServices.CanRead(entity, loggedUser))
                {
                    return StatusCode(403, "Current user cannot read this object!");
                }

                if(entity == null)
                {
                    return NotFound($"Appointment not found with id='{id}'!");
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
        [Authorize]
        public IActionResult UpdateState(int id, Appointment.States state)
        {
            try
            {
                Appointment entity = QueryAll().FirstOrDefault(x => x.Id == id);

                if (entity == null)
                {
                    return NotFound($"Appointment not found with id='{id}'!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (!_servicesManager.AppointmentServices.CanUpdateState(entity, loggedUser, state))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái, quyền và dữ liệu đầu vào!");
                }

                entity.State = state;
                _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);
            }
            catch (Exception ex)
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
        //[HttpPut("{id}")]
        //[Authorize(Roles = nameof(UserType.Receptionist) + "," + nameof(UserType.Administrator))]
        //public IActionResult Accept(int id)
        //{
        //    try
        //    {
        //        Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

        //        if (entity == null)
        //        {
        //            return NotFound("Appointment not found!");
        //        }

        //        BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
        //        if(!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
        //        {
        //            return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
        //        }

        //        entity.State = Appointment.States.Accept;
        //        _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
        //        _servicesManager.DbContext.SaveChanges();

        //        AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

        //        return Ok(entityDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

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
        //[HttpPut("{id}")]
        //[Authorize(Roles = nameof(UserType.Receptionist) + "," + nameof(UserType.Administrator))]
        //public IActionResult Cancel(int id)
        //{
        //    try
        //    {
        //        Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

        //        if (entity == null)
        //        {
        //            return NotFound("Truyền sai id rồi => Appointment not found!");
        //        }

        //        BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
        //        if (!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
        //        {
        //            return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
        //        }

        //        entity.State = Appointment.States.Cancel;
        //        _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
        //        _servicesManager.DbContext.SaveChanges();

        //        AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

        //        return Ok(entityDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

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
        //[HttpPut("{id}")]
        //[Authorize(Roles = nameof(UserType.Doctor) + "," + nameof(UserType.Administrator))]
        //public IActionResult Doing(int id)
        //{
        //    try
        //    {
        //        Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

        //        if (entity == null)
        //        {
        //            return NotFound("Truyền sai id rồi => Appointment not found!");
        //        }

        //        BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
        //        if (!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
        //        {
        //            return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
        //        }

        //        entity.State = Appointment.States.Doing;
        //        _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
        //        _servicesManager.DbContext.SaveChanges();

        //        AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

        //        return Ok(entityDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

        //[HttpPut("{id}")]
        //[Authorize(Roles = nameof(UserType.Doctor) + "," + nameof(UserType.Administrator))]
        //public IActionResult Transfer(int id)
        //{
        //    try
        //    {
        //        Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

        //        if (entity == null)
        //        {
        //            return NotFound($"Appointment not found with id='{id}'!");
        //        }

        //        BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
        //        if (!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
        //        {
        //            return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
        //        }

        //        entity.State = Appointment.States.Transfer;
        //        _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
        //        _servicesManager.DbContext.SaveChanges();

        //        AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

        //        return Ok(entityDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

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
        //[HttpPut("{id}")]
        //[Authorize(Roles = nameof(UserType.Doctor) + "," + nameof(UserType.Administrator))]
        //public IActionResult Complete(int id)
        //{
        //    try
        //    {
        //        Appointment entity = QueryAll().FirstOrDefault(apm => apm.Id == id);

        //        if (entity == null)
        //        {
        //            return NotFound("Truyền sai id rồi => Appointment not found!");
        //        }

        //        BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
        //        if (!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
        //        {
        //            return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
        //        }

        //        entity.State = Appointment.States.Complete;
        //        _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
        //        _servicesManager.DbContext.SaveChanges();

        //        AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

        //        return Ok(entityDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}

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
                if (!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
                }

                Document document = _servicesManager.AutoMapper.Map<Document>(requestModel);
                if(string.IsNullOrWhiteSpace(document.Title))
                {
                    document.Title = "Doctor document for appointment";
                }

                document.Tag = Document.DocumentTags.Doctor;

                IFormFile file = requestModel.DocumentFile;

                // < Xử lý file >
                if (!file.ContentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                {
                    return BadRequest("File file must be *.docx format!");
                }

                document.File = new FileMedia()
                {
                    Category = FileMedia.FileCategory.AppointmentDocument
                };

                string filename = $"apm_{entity.Id}_Doctor+{DateTime.Now.Ticks}" + Path.GetExtension(file.FileName);
                var result = await _servicesManager.DropboxServices.UploadAsync(file, filename);

                document.File.FilePath = result.UploadPath;

                entity.Documents.Add(document);
                _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();
                // </ Xử lý file >

                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Allow patients adding a document for their appointment
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
        [Authorize(Roles = nameof(UserType.Patient) + "," + nameof(UserType.Administrator))]
        public async Task<IActionResult> PatientAddDocumentAsync([FromForm] AddDocumentModel requestModel)
        {
            try
            {

                Appointment entity = QueryAll().FirstOrDefault(item => item.Id == requestModel.AppointmentId);

                if (entity == null)
                {
                    return NotFound("Appointment not found! Kiểm tra lại 'appoinmentId'!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (!_servicesManager.AppointmentServices.CanWrite(entity, loggedUser))
                {
                    return StatusCode(403, "Không thể thực hiện! Kiểm tra lại trạng thái và quyền!");
                }

                Document document = _servicesManager.AutoMapper.Map<Document>(requestModel);
                if (string.IsNullOrWhiteSpace(document.Title))
                {
                    document.Title = "Adding document by Patient";
                }

                document.Tag = Document.DocumentTags.Patient;

                IFormFile file = requestModel.DocumentFile;

                // < Xử lý file >
                if (!file.ContentType.Equals("application/vnd.openxmlformats-officedocument.wordprocessingml.document"))
                {
                    return BadRequest("File file must be *.docx format!");
                }

                document.File = new FileMedia()
                {
                    Category = FileMedia.FileCategory.AppointmentDocument
                };

                string filename = $"apm_{entity.Id}Patient+{DateTime.Now.Ticks}" + Path.GetExtension(file.FileName);
                var result = await _servicesManager.DropboxServices.UploadAsync(file, filename);

                document.File.FilePath = result.UploadPath;

                entity.Documents.Add(document);
                _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();
                // </ Xử lý file >

                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);

                return Ok(entityDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Remove a document by Reception or Admin
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(UserType.Receptionist) + "," + nameof(UserType.Administrator))]
        public IActionResult RemoveDocument(int id) 
        {
            try
            {
                Document entity = _servicesManager.DbContext.Documents.Find(id);

                if(entity == null)
                {
                    return NotFound("File not found!");
                }

                _servicesManager.DbContext.Documents.Remove(entity);
                _servicesManager.DbContext.SaveChanges();

                return Ok($"Removed File '{entity.Title}' of Appointment '{entity.AppointmentId}'");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        [Authorize(Roles = nameof(UserType.Receptionist) + "," + nameof(UserType.Administrator))]
        public IActionResult Update(UpdateAppointment requestModel)
        {
            try
            {
                Appointment entity = _servicesManager.DbContext.Appointments.Find(requestModel.Id);
                if(entity == null)
                {
                    return NotFound("Appointment not found!");
                }

                _servicesManager.AutoMapper.Map<UpdateAppointment, Appointment>(requestModel, entity);

                _servicesManager.DbContext.Entry(entity).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                entity = QueryAll().FirstOrDefault(a => a.Id == entity.Id);
                AppointmentDTO entityDTO = _servicesManager.AutoMapper.Map<AppointmentDTO>(entity);
                return Ok(entityDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
