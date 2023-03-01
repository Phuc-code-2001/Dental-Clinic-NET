using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Notifications;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Services.Notifications;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
                Paginated<Notification> paginated = new Paginated<Notification>(queryFiltered, filter.PageIndex, filter.PageSize);

                var dtos = _servicesManager.AutoMapper.Map<NotificationDTO[]>(paginated.Items.ToArray());

                return Ok(dtos);
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

    }
}
