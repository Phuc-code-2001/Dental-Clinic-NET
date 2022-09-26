using AutoMapper;
using DataLayer.Schemas;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Facebooks.Services;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Serializers;
using Dental_Clinic_NET.API.Utils;
using ImageProcessLayer.ImageKitResult;
using ImageProcessLayer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
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
        private IMapper _mapper;
        private UserManager<BaseUser> _userManager;
        private FacebookServices _facebookServices;
        private ImageKitServices _imageKitServices;

        public HelperController(UserManager<BaseUser> userManager, ImageKitServices imageKitServices, FacebookServices facebookServices, IMapper mapper)
        {
            _userManager = userManager;
            _imageKitServices = imageKitServices;
            _facebookServices = facebookServices;
            _mapper = mapper;
        }

        [HttpDelete]
        public IActionResult DeleteAccountNullFullNameAsync()
        {
            try
            {
                int count = 0;
                List<BaseUser> users = _userManager.Users.Where(u => u.FullName == null).ToList();
                List<Task<IdentityResult>> deleteTasks = new List<Task<IdentityResult>>();
                foreach(var user in users)
                {
                    deleteTasks.Add(_userManager.DeleteAsync(user));
                };

                foreach(var task in deleteTasks)
                {
                    var res = task.Result;
                    if(res.Succeeded)
                    {
                        count++;
                    }
                }

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
        [EnableQuery]
        public IActionResult ViewAllAccount()
        {
            try
            {

                var users = _userManager.Users.AsEnumerable().Select(user =>
                {
                    PermissionOnBaseUser permission = new PermissionOnBaseUser(user, user);
                    UserSerializer serializer = new UserSerializer(permission);
                    return serializer.Serialize(user =>
                    {
                        return _mapper.Map<UserDTO>(user);
                    });
                });

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
