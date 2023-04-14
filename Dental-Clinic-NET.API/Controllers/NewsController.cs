using DataLayer.Domain;
using Dental_Clinic_NET.API.DTOs;
using Dental_Clinic_NET.API.Models.Posts;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        ServicesManager _servicesManager { get; set; }

        public NewsController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Administrator) + "," + nameof(UserType.Receptionist))]
        public IActionResult Create([FromForm] CreatePost form)
        {
            try
            {
                Post post = _servicesManager.AutoMapper.Map<CreatePost, Post>(form);
                post.Services = _servicesManager.DbContext.Services.Where(x => form.ServicesId.Contains(x.Id)).ToList();

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                post.Creator = loggedUser;

                _servicesManager.DbContext.Posts.Add(post);
                _servicesManager.DbContext.SaveChanges();

                PostDTO dtos = _servicesManager.AutoMapper.Map<Post, PostDTO>(post);

                return Ok(dtos);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] PostFilter filter)
        {
            try
            {
                IQueryable<Post> posts = _servicesManager.DbContext.Posts
                            .Include(x => x.Creator)
                            .Include(x => x.Services);

                posts = filter.GetFilteredQuery(posts);

                var paginated = new Paginated<Post>(posts, filter.Page, filter.PageSize);
                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<PostDTO[]>(items.ToArray()));

                return Ok(dataset);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                Post post = _servicesManager.DbContext.Posts
                            .Include(x => x.Creator)
                            .Include(x => x.Services).FirstOrDefault(x => x.Id == id);

                if(post == null)
                {
                    return NotFound($"Post not found with id='{id}'");
                }

                PostDTO dto = _servicesManager.AutoMapper.Map<PostDTO>(post);

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(UserType.Administrator) + "," + nameof(UserType.Receptionist))]
        public IActionResult Update(int id, [FromForm] UpdatePost form)
        {
            try
            {
                Post post = _servicesManager.DbContext.Posts
                            .Include(x => x.Services)
                            .FirstOrDefault(x => x.Id == id);
                if(post == null)
                {
                    return NotFound("Post not found!");
                }

                _servicesManager.AutoMapper.Map<UpdatePost, Post>(form, post);

                post.Services = _servicesManager.DbContext.Services
                                .Where(x => form.ServicesId.Contains(x.Id))
                                .ToList();

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);
                post.Creator = loggedUser;

                _servicesManager.DbContext.Posts.Update(post);
                _servicesManager.DbContext.SaveChanges();

                PostDTO dtos = _servicesManager.AutoMapper.Map<Post, PostDTO>(post);

                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete]
        [Authorize(Roles = nameof(UserType.Administrator) + "," + nameof(UserType.Receptionist))]
        public IActionResult Delete(int id)
        {
            try
            {
                Post post = _servicesManager.DbContext.Posts.FirstOrDefault(x => x.Id == id);
                if (post == null)
                {
                    return NotFound("Post not found!");
                }

                _servicesManager.DbContext.Posts.Remove(post);
                _servicesManager.DbContext.SaveChanges();

                return Ok("Delete success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
