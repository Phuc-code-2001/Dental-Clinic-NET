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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IMapper _mapper;
        private UserManager<BaseUser> _userManager;
        private ImageKitServices _imageKitServices;

        private ServicesManager _servicesManager;

        public UserController(UserManager<BaseUser> userManager, ImageKitServices imageKitServices, IMapper mapper, ServicesManager servicesManager)
        {
            _userManager = userManager;
            _imageKitServices = imageKitServices;
            _mapper = mapper;
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

                if (_userManager.Users.Any(user => user.Type == UserType.Administrator))
                {
                    return BadRequest("Superuser already exist...");
                }

                BaseUser user = inputInfo.ToBaseUser_NotIncludePassword();
                user.Type = UserType.Administrator;
                IdentityResult result = await _userManager.CreateAsync(user, inputInfo.Password);

                if (result.Succeeded)
                {
                    return Ok(_mapper.Map<UserDTO>(user));
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
        [EnableQuery]
        public async Task<IActionResult> GetAuthorizeAsync()
        {
            try
            {
                BaseUser loggedUser = await _userManager.FindByIdAsync(User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? "");
                UserSerializer serializer = new UserSerializer(new PermissionOnBaseUser(loggedUser, loggedUser));

                return Ok(serializer.Serialize(user =>
                {
                    return _mapper.Map<UserDTO>(user);
                }));
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
        ///     200: { string: newImage, UserDTO: user }
        /// </returns>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateAvatarAsync(string userId, IFormFile image)
        {
            try
            {
                BaseUser requiredUser = await _userManager.FindByIdAsync(userId);
                BaseUser loginUser = await _userManager.FindByNameAsync(User.Identity.Name);

                PermissionOnBaseUser permission = new PermissionOnBaseUser(loginUser, requiredUser);

                if (!permission.IsAdmin && !permission.IsOwner)
                {
                    return Unauthorized("Cann't do this operation");
                }

                if (_imageKitServices.IsImage(image))
                {
                    var result = await _imageKitServices.UploadImageAsync(image, image.FileName);
                    if (requiredUser.ImageAvatarId != null)
                    {
                        await _imageKitServices.DeleteImageAsync(requiredUser.ImageAvatarId);
                    }
                    requiredUser.ImageURL = result.URL;
                    requiredUser.ImageAvatarId = result.ImageId;
                    await _userManager.UpdateAsync(requiredUser);

                    UserSerializer serializer = new UserSerializer(permission);
                    return Ok(new
                    {
                        newImage = requiredUser.ImageURL,
                        user = serializer.Serialize(user => _mapper.Map<UserDTO>(user))
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
        [Authorize(Roles = "Administrator")]
        [EnableQuery(PageSize = 10)]
        public IActionResult GetUsers(int page = 1)
        {
            try
            {
                var users = _userManager.Users.ToList();
                var usersDTO = users.Select(user => _mapper.Map<UserDTO>(user)).ToList();

                Paginated<UserDTO> paginatedUsers = new Paginated<UserDTO>(usersDTO.AsQueryable(), page);


                return Ok(new
                {
                    page = page,
                    per_page = paginatedUsers.PageSize,
                    total = paginatedUsers.ColectionCount,
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
        ///  Change role of specific user
        /// </summary>
        /// <param name="request">Info need to change role</param>
        /// <returns>
        ///     404: user not found
        ///     400: something went wrong so cannot update
        ///     500: Server handle error
        ///     200: Update success
        /// </returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ChangeRole(ChangeRoleModel request)
        {
            try
            {
                BaseUser user = await _userManager.FindByIdAsync(request.UserId);
                if (user == null) return NotFound("User not found");

                user.Type = request.RoleId;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors;
                    return BadRequest(errors);
                }

                UserDTO userDTO = _servicesManager.AutoMapper.Map<UserDTO>(user);
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
                BaseUser requiredUser = await _userManager.FindByIdAsync(request.userId);
                BaseUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, requiredUser);

                if (!permission.IsOwner && !permission.IsAdmin)
                {
                    return StatusCode(StatusCodes.Status403Forbidden);
                }

                if (!String.IsNullOrWhiteSpace(request.FullName)) requiredUser.FullName = request.FullName;
                if (request.BirthDate != null) requiredUser.BirthDate = request.BirthDate.Value;
                if (!String.IsNullOrWhiteSpace(request.Address)) requiredUser.Address = request.Address;
                if (!String.IsNullOrWhiteSpace(request.Gender)) requiredUser.Gender = request.Gender;

                IdentityResult updateResult = await _userManager.UpdateAsync(requiredUser);

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
        ///   Update password for specific user
        /// </summary>
        /// <param name="request">Info needed to update password</param>
        /// <returns>
        ///     404: Not found user
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
                BaseUser requiredUser = await _userManager.FindByIdAsync(request.userId);
                if(requiredUser == null)
                {
                    return NotFound("Truyền sai userId rồi ba");
                }

                BaseUser loggedUser = await _userManager.FindByNameAsync(User.Identity.Name);
                PermissionOnBaseUser permission = new PermissionOnBaseUser(loggedUser, requiredUser);
                if(!permission.IsAdmin && !permission.IsOwner)
                {
                    return StatusCode(403);
                }

                IdentityResult updateResult = await _userManager.ChangePasswordAsync(requiredUser, request.OldPassword, request.NewPassword);

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

    }
}
