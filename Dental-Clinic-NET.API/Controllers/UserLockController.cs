using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Users.UserLock;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserLockController : ControllerBase
    {
        ServicesManager _serviceManager;

        public UserLockController(ServicesManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> LockAsync(CreateUserLock form)
        {
            try
            {
                BaseUser requiredUser = await _serviceManager.UserManager.FindByIdAsync(form.UserId);

                if(requiredUser == null)
                {
                    return NotFound("User not found!");
                }

                UserLock userLock = _serviceManager.DbContext.UserLocks.Find(requiredUser.Id);
                if(userLock == null)
                {
                    userLock = _serviceManager.AutoMapper.Map<UserLock>(form);
                    _serviceManager.DbContext.Entry(userLock).State = EntityState.Added;
                }
                else
                {
                    _serviceManager.AutoMapper.Map<CreateUserLock, UserLock>(form, userLock);
                    _serviceManager.DbContext.Entry(userLock).State = EntityState.Modified;
                }

                userLock.IsLocked = true;
                _serviceManager.DbContext.SaveChanges();

                return Ok($"The user '{requiredUser.UserName}' is lock until '{userLock.Expired.ToShortTimeString()}'");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> UnlockAsync(UnlockUserForm form)
        {
            try
            {
                BaseUser requiredUser = await _serviceManager.UserManager.FindByIdAsync(form.UserId);
                
                if(requiredUser == null)
                {
                    return NotFound("User not found!");
                }

                UserLock userLock = _serviceManager.DbContext.UserLocks.Find(requiredUser.Id);
                if(userLock == null || !userLock.IsLocked || userLock.Expired < DateTime.Now)
                {
                    return BadRequest("This user is available");
                }

                userLock.IsLocked = false;
                _serviceManager.DbContext.Entry(userLock).State = EntityState.Modified;
                _serviceManager.DbContext.SaveChanges();

                return Ok($"The user '{requiredUser.UserName}' is unlock");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
