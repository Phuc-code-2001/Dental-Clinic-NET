using DataLayer.Schemas;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Serializers;
using ImageProcessLayer.ImageKitResult;
using ImageProcessLayer.Services;
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
        private FacebookServices _facebookServices;
        private ImageKitServices _imageKitServices;

        public HelperController(UserManager<BaseUser> userManager, ImageKitServices imageKitServices, FacebookServices facebookServices)
        {
            _userManager = userManager;
            _imageKitServices = imageKitServices;
            _facebookServices = facebookServices;
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

        [HttpPost]
        public async Task<IActionResult> TestPostImageAsync(IFormFile file)
        {
            try
            {
                bool validFileImage = _imageKitServices.IsImage(file);
                if (!validFileImage) return BadRequest("Input must be a image");

                string filename = file.FileName;
                ImageKitUploadResult result = await _imageKitServices.UploadImageAsync(file, filename);

                return Ok(result);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete]
        public async Task<IActionResult> TestDeleteImageAsync(string imageId)
        {
            try
            {
                
                await _imageKitServices.DeleteImageAsync(imageId);
                return Ok($"Delete image with id={imageId} success");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        public IActionResult ViewAllAccount()
        {
            try
            {
                var users = _userManager.Users.Select(user => new UserSerializer(null, user));

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
