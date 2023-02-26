using DataLayer.Domain;
using Dental_Clinic_NET.API.Models.Users.UserLock;
using Dental_Clinic_NET.API.Services;
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

                UserLock userLock = _serviceManager.AutoMapper.Map<CreateUserLock, UserLock>(form);
                userLock.IsLocked = true;

                _serviceManager.DbContext.UserLocks.Add(userLock);
                _serviceManager.DbContext.SaveChanges();

                string strLockTo = $"{userLock.Expired.ToShortTimeString()} {userLock.Expired.ToShortDateString()}";
                return Ok($"The user '{requiredUser.UserName}' is lock until '{strLockTo}'");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
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

                UserLock userLock = await _serviceManager.DbContext.UserLocks
                    .OrderBy(e => e.TimeCreated)
                    .LastOrDefaultAsync(ulock => ulock.BaseUserId == requiredUser.Id);

                if(userLock == null || !userLock.IsLocked || userLock.Expired < DateTime.Now)
                {
                    return BadRequest("This user is available");
                }

                userLock = new UserLock()
                {
                    BaseUserId = requiredUser.Id,
                    IsLocked = false,
                };

                _serviceManager.DbContext.UserLocks.Add(userLock);
                _serviceManager.DbContext.SaveChanges();

                return Ok($"The user '{requiredUser.UserName}' is unlock");

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> UserLockHistories(string userId)
        {
            try
            {
                BaseUser requiredUser = await _serviceManager.DbContext.Users.
                    Include(u => u.UserLocks)
                    .FirstOrDefaultAsync(u => u.Id == userId);
                if(requiredUser == null )
                {
                    return NotFound("User not found!");
                }

                var LockHistories = requiredUser.UserLocks;

                return Ok(LockHistories);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
