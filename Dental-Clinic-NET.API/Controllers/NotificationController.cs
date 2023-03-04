using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Notifications;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        ServicesManager _servicesManager;

        public NotificationController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Get([FromQuery] NotificationFilter filter)
        {
            try
            {
                var queryAll = _servicesManager.NotificationServices.QueryAll();

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if(loggedUser.Type != UserType.Administrator)
                {
                    filter.ReceiverId = loggedUser.Id;
                    queryAll = queryAll.Where(obj => obj.Hidden == false);
                }

                var queryFiltered = filter.Filter(queryAll);
                Paginated<Notification> paginated = new Paginated<Notification>(queryFiltered, filter.Page, filter.PageSize);

                dynamic data = paginated.GetData(items => _servicesManager.AutoMapper.Map<NotificationDTO[]>(items.ToArray()));
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                Notification entity = await _servicesManager.NotificationServices.QueryAll()
                    .FirstOrDefaultAsync(e => e.Id == id);

                if(entity == null)
                {
                    return NotFound("Object not found!");
                }

                NotificationDTO dto = _servicesManager.AutoMapper.Map<NotificationDTO>(entity);
                return Ok(dto);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/Click")]
        [Authorize]
        public async Task<IActionResult> ClickAsync(int id) 
        {
            try
            {
                Notification notification = await _servicesManager.DbContext.Notifications
                    .FirstOrDefaultAsync(e => e.Id == id);

                if(notification == null)
                {
                    return NotFound("Notification not found.");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if(loggedUser.Id != notification.Receiver.Id)
                {
                    return StatusCode(403, "Bad request! Only allow receiver click!");
                }

                notification.Clicked = true;
                _servicesManager.DbContext.Notifications.Update(notification);
                _servicesManager.DbContext.SaveChanges();

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}/Hide")]
        public async Task<IActionResult> HideAsync(int id)
        {
            try
            {
                Notification notification = await _servicesManager.DbContext.Notifications
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (loggedUser.Id != notification.Receiver.Id)
                {
                    return StatusCode(403, "Bad request! Only allow receiver hide!");
                }

                notification.Hidden = true;
                _servicesManager.DbContext.Notifications.Update(notification);
                _servicesManager.DbContext.SaveChanges();

                return Ok();
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                Notification notification = await _servicesManager.DbContext.Notifications
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (notification == null)
                {
                    return NotFound("Notification not found.");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                if (loggedUser.Type != UserType.Administrator)
                {
                    return StatusCode(403, "Bad request! Only allow admin delete notification!");
                }

                _servicesManager.DbContext.Notifications.Remove(notification);
                _servicesManager.DbContext.SaveChanges();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
