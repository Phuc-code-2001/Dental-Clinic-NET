using DataLayer.Schemas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {

        private UserManager<BaseUser> _userManager;

        public HelperController(UserManager<BaseUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAccountNullFullNameAsync()
        {
            try
            {
                int count = 0;
                List<BaseUser> users = _userManager.Users.Where(u => u.FullName == null).ToList();
                foreach(var user in users)
                {
                    if((await _userManager.DeleteAsync(user)).Succeeded)
                    {
                        count++;
                    }
                };

                return Ok(new {
                    count
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
