using AutoMapper;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Users;
using Dental_Clinic_NET.API.Permissions;
using Dental_Clinic_NET.API.Serializers;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using ImageProcessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private ServicesManager _servicesManager;

        public UserController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        /// <summary>
        ///     Create account with role=Administrator when enable application
        /// </summary>
        /// <param name="inputInfo">Account Info</param>
        /// <returns>
        ///     400: Superuser already exist || Info invalid
        ///     200: Create success
        ///     500: Server Handle Error
        ///     
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateSuperUserAsync(CreateSuperUserModel inputInfo)
        {

            try
            {

                if (_servicesManager.UserManager.Users.Any(user => user.Type == UserType.Administrator))
                {
                    return BadRequest("Superuser already exist...");
                }

                BaseUser user = new BaseUser()
                {
                    UserName = inputInfo.UserName,
                    FullName = inputInfo.FullName,
                    Type = UserType.Administrator,
                    PusherChannel = _servicesManager.UserServices.GenerateUniqueUserChannel(),
                };
                
                IdentityResult result = await _servicesManager.UserManager
                    .CreateAsync(user, inputInfo.Password);

                if (result.Succeeded)
                {
                    return Ok(_servicesManager.AutoMapper.Map<UserDTO>(user));
                }

                var errors = result.Errors.Select(er => new { er.Code, er.Description });

                return BadRequest(new
                {
                    errorCount = errors.Count(),
                    errors = errors,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Get User information by JWT after login
        /// </summary>
        /// <returns>
        ///     UserDTO: User information
        /// </returns>
        [HttpGet]
        [Authorize]
        public IActionResult GetAuthorizeAsync()
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                return Ok(_servicesManager.AutoMapper.Map<UserDTO>(loggedUser));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        /// Update avatar for User
        /// </summary>
        /// <param name="userId">Id of User</param>
        /// <param name="image">Image file</param>
        /// <returns>
        ///     401: not is_admin, not is_owner, not is_authenticated
        ///     400: Image file invalid
        ///     500: ImageKit server error || Server handle error
        ///     200: { string: newImage, UserDTO: requiredUser }
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateAvatarAsync([FromForm] string userId, [FromForm] IFormFile image)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByIdAsync(userId);
                
                if (requiredUser == null)
                {
                    return NotFound($"User not found. Id='{userId}'");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, requiredUser);

                if (!permission.IsAdmin && !permission.IsOwner)
                {
                    return Unauthorized("Cann't do this operation");
                }

                if (_servicesManager.ImageKitServices.IsImage(image))
                {
                    string imgName = "avatar_" + requiredUser.UserName;
                    var result = await _servicesManager.ImageKitServices.UploadImageAsync(image, imgName);
                    
                    if (requiredUser.ImageAvatarId != null)
                    {
                        await _servicesManager.ImageKitServices.DeleteImageAsync(requiredUser.ImageAvatarId);
                    }
                    requiredUser.ImageURL = result.URL;
                    requiredUser.ImageAvatarId = result.ImageId;
                    await _servicesManager.UserManager.UpdateAsync(requiredUser);

                    UserSerializer serializer = new UserSerializer(permission);
                    return Ok(new
                    {
                        newImage = requiredUser.ImageURL,
                        user = serializer.Serialize(user => _servicesManager.AutoMapper.Map<UserDTO>(user))
                    });
                }

                return BadRequest("File must be image");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Require administrator role. List all users within ODATA, each page include max 10 users
        /// </summary>
        /// <returns>
        ///     200: Query success
        ///     500: Server handle error
        /// </returns>
        [HttpGet]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public IActionResult GetUsers(int page = 1)
        {
            try
            {
                var users = _servicesManager.UserManager.Users.ToList();
                var usersDTO = users.Select(user => _servicesManager.AutoMapper.Map<UserDTO>(user)).ToList();

                Paginated<UserDTO> paginatedUsers = new Paginated<UserDTO>(usersDTO.AsQueryable(), page);


                return Ok(new
                {
                    page = page,
                    per_page = paginatedUsers.PageSize,
                    total = paginatedUsers.QueryCount,
                    total_pages = paginatedUsers.PageCount,
                    data = paginatedUsers.Items
                });

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///  Change role of specific requiredUser
        /// </summary>
        /// <param name="request">Info need to change role</param>
        /// <returns>
        ///     404: requiredUser not found
        ///     400: something went wrong so cannot update
        ///     500: Server handle error
        ///     200: Update success
        /// </returns>
        [HttpPut]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> ChangeRole(ChangeRoleModel request)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByIdAsync(request.UserId);
                if (requiredUser == null)
                {
                    return NotFound($"User not found, Id='{request.UserId}'");
                }

                requiredUser.Type = request.RoleId;
                var result = await _servicesManager.UserManager.UpdateAsync(requiredUser);

                if (!result.Succeeded)
                {
                    var errors = result.Errors;
                    return BadRequest(errors);
                }

                UserDTO userDTO = _servicesManager.AutoMapper.Map<UserDTO>(requiredUser);
                return Ok(userDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Update User information by owner or admin
        /// </summary>
        /// <param name="request">UpdateUserInfo(FullName, BirthDate, Address, Gender)</param>
        /// <returns>
        ///     403: Forbidden (Not isAdmin and IsOwner)
        ///     400: Fields invalid
        ///     500: Server handle error
        ///     200: Update success
        /// </returns>
        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateAsync(UpdateUserModel request)
        {

            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByIdAsync(request.userId);
                
                if(requiredUser == null)
                {
                    return NotFound($"User not found. Id='{request.userId}'");
                }
                
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, requiredUser);

                if (!permission.IsOwner && !permission.IsAdmin)
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                _servicesManager.AutoMapper.Map<UpdateUserModel, BaseUser>(request, requiredUser);

                IdentityResult updateResult = await _servicesManager.UserManager.UpdateAsync(requiredUser);

                if (updateResult.Succeeded)
                {
                    UserDTO userDTO = new UserSerializer(permission).Serialize((user) =>
                    {
                        return _servicesManager.AutoMapper.Map<UserDTO>(user);
                    });
                    return Ok(userDTO);
                }

                var errors = updateResult.Errors.ToList();
                return BadRequest(errors);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        /// <summary>
        ///   Update password for specific requiredUser
        /// </summary>
        /// <param name="request">Info needed to update password</param>
        /// <returns>
        ///     404: Not found requiredUser
        ///     403: Forbiden
        ///     400: an invalid or error while update
        ///     500: Server handle error
        ///     200: Update success
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdatePasswordAsync(UpdatePasswordModel request)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByIdAsync(request.userId);
                if(requiredUser == null)
                {
                    return NotFound($"User not found. Id='{request.userId}'");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, requiredUser);
                if(!permission.IsAdmin && !permission.IsOwner)
                {
                    return StatusCode(403);
                }

                IdentityResult updateResult = await _servicesManager.UserManager
                    .ChangePasswordAsync(requiredUser, request.OldPassword, request.NewPassword);

                if(updateResult.Succeeded)
                {
                    return Ok("Update password success");
                }

                var errors = updateResult.Errors;

                return BadRequest(errors);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        [Authorize]
        public async Task<IActionResult> GetAsync(string userId)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByIdAsync(userId);
                if (requiredUser == null)
                {
                    return NotFound($"User not found. Id='{userId}'");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, requiredUser);
                if (!permission.IsAdmin && !permission.IsOwner)
                {
                    return StatusCode(403);
                }

                UserDTO userDTO = new UserSerializer(permission).Serialize((user) =>
                {
                    return _servicesManager.AutoMapper.Map<UserDTO>(user);
                });

                return Ok(userDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{userId}")]
        [Authorize(Roles = nameof(UserType.Administrator))]
        public async Task<IActionResult> DeleteAsync(string userId)
        {
            try
            {
                BaseUser requiredUser = await _servicesManager.UserManager.FindByIdAsync(userId);
                if (requiredUser == null)
                {
                    return NotFound($"User not found. Id='{userId}'");
                }

                await _servicesManager.UserManager.DeleteAsync(requiredUser);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
