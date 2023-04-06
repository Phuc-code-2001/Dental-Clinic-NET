using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.FeedBacks;
using Dental_Clinic_NET.API.Models.Services;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class FeedBackController : ControllerBase
    {
        ServicesManager _servicesManager;

        public FeedBackController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Patient))]
        public async Task<IActionResult> CreateAsync(FeedBackCreation formData)
        {
            try
            {
                Appointment appointment = await _servicesManager.DbContext.Appointments
                    .FirstOrDefaultAsync(x => x.Id == formData.AppointmentId);

                if (appointment == null)
                {
                    return NotFound("Appoiment not found!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                var permission = new PermissionOnAppointment(loggedUser, appointment);
                if(!permission.IsOwner)
                {
                    return StatusCode(403, "You don't have permission to feedback!");
                }

                if(appointment.State != Appointment.States.Complete)
                {
                    return BadRequest("Appointment is not complete");
                }

                FeedBack feedBack = new FeedBack()
                {
                    AppointmentId = appointment.Id,
                    ServiceId = appointment.ServiceId,
                    User = loggedUser,
                    Content = formData.Content,
                    RatingPoint = formData.RatingPoint.Value
                };

                _servicesManager.DbContext.FeedBacks.Add(feedBack);
                _servicesManager.DbContext.SaveChanges();
                FeedBackDTO feedBackView = _servicesManager.AutoMapper.Map<FeedBackDTO>(feedBack);

                return Ok(feedBackView);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetServiceFeedBacks(int serviceId, [FromQuery] PageFilter filter)
        {
            try
            {
                IQueryable<FeedBack> feedbacks = _servicesManager.DbContext.FeedBacks
                    .Include(x => x.User)
                    .Where(x => x.ServiceId == serviceId);

                FeedbacksOfServiceView view = new FeedbacksOfServiceView();

                view.AverageRatingPoint = feedbacks.Average(x => x.RatingPoint);
                view.Total = feedbacks.Count();
                
                view.Percentages = feedbacks.AsEnumerable().GroupBy(x => x.RatingPoint)
                    .ToDictionary(x => x.Key, x => x.Count() / (float) view.Total);

                Paginated<FeedBack> paginated = new Paginated<FeedBack>(feedbacks, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<FeedBackDTO[]>(items));

                view.Items = dataset;

                return Ok(view);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetRankServices() 
        {
            try
            {
                IQueryable<Service> queries = _servicesManager.DbContext
                    .Services.Where(service => service.IsPublic);

                ServiceDTOLite[] services = _servicesManager.AutoMapper.Map<ServiceDTOLite[]>(queries.ToArray());

                List<ServiceRanking> ranking = new List<ServiceRanking>();
                foreach(ServiceDTOLite service in services)
                {
                    IQueryable<FeedBack> feedbacks = _servicesManager.DbContext
                        .FeedBacks.Where(x => x.ServiceId == service.Id);
                    int count = feedbacks.Count();
                    float avg = count > 0 ? feedbacks.Average(x => x.RatingPoint) : 0;

                    ranking.Add(new ServiceRanking
                    {
                        ServiceInfo = service,
                        AveragePoint = avg,
                        FeedBackCount = count
                    });
                }

                ranking = ranking.OrderByDescending(x => x.AveragePoint).ToList();

                return Ok(ranking);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAppointmentFeedback(int AppointmentId)
        {
            try
            {
                Appointment appointment = _servicesManager.DbContext.Appointments.FirstOrDefault(x => x.Id == AppointmentId);

                if(appointment == null)
                {
                    return NotFound($"Appointment with id='{AppointmentId}' not found!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                PermissionOnAppointment permission = new PermissionOnAppointment(loggedUser, appointment);

                if(!(permission.IsOwner || permission.IsAdmin || loggedUser.Type == UserType.Receptionist))
                {
                    return Unauthorized("Do not have permission!");
                }

                FeedBack feedback = _servicesManager.DbContext.FeedBacks
                    .Include(x => x.User)
                    .FirstOrDefault(x => x.AppointmentId == AppointmentId);

                if(feedback == null)
                {
                    return NotFound();
                }

                FeedBackDTO jsonData = _servicesManager.AutoMapper.Map<FeedBackDTO>(feedback);
                return Ok(jsonData);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    
    }
}
