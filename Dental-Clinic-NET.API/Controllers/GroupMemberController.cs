using DataLayer.Schemas;
using DataLayer.Services.GroupMembers;
using Dental_Clinic_NET.API.Controllers.Helpers;
using Dental_Clinic_NET.API.Models.GroupMemberModels;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class GroupMemberController : ControllerBase, IPaginatedController
    {

        private GroupMemberServices _groupMemberServices;

        public GroupMemberController()
        {
            // Dependency injection
            _groupMemberServices = new GroupMemberServices();
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {   
            try
            {
                var groupMember = _groupMemberServices.GetById(id);
                if (groupMember == null) return NotFound($"Not found group member with id={id}");

                return Ok(groupMember);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Insert(CreateGroupMemberModel createModel)
        {
            try
            {
                int id = _groupMemberServices.Insert(createModel.GetEntity());
                return GetById(id);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPut]
        public IActionResult Update(UpdateGroupMemberModel updateModel)
        {
            try
            {
                var member = _groupMemberServices.GetById(updateModel.Id);
                if (member == null) return NotFound($"Not found group member with id={updateModel.Id}");
                _groupMemberServices.Update(member);
                return GetById(member.Id);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpDelete("{id}")]
        public IActionResult Remove(int id)
        {
            try
            {
                var member = _groupMemberServices.GetById(id);
                if(member == null) return NotFound("Not found group member with id={id}");

                _groupMemberServices.Remove(member);
                return Ok($"Remove success member id={id}");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_groupMemberServices.GetAll());
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetPage(int? pageIndex)
        {

            try
            {

                if (pageIndex == null) return BadRequest("Missing pageIndex");
                if (pageIndex.Value <= 0) return BadRequest("Invalid params: pageIndex must be positive integer");

                IQueryable<GroupMember> groupMembers = _groupMemberServices.GetAll();
                Paginated<GroupMember> paginatedGroupMembers = new (groupMembers, pageIndex.Value);

                string url = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/api/{ControllerContext.ActionDescriptor.ControllerName}/{ControllerContext.ActionDescriptor.ActionName}";

                string previousLink = paginatedGroupMembers.HasPrevious ? url + $"?pageIndex={pageIndex - 1}" : null;
                string nextLink = paginatedGroupMembers.HasNext ? url + $"?pageIndex={pageIndex + 1}" : null;

                return Ok(new
                {
                    page = pageIndex,
                    per_page = paginatedGroupMembers.PageSize,
                    total = paginatedGroupMembers.ColectionCount,
                    total_pages = paginatedGroupMembers.PageCount,
                    data = paginatedGroupMembers.Items,
                    previous = previousLink,
                    next = nextLink,
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
    }
}
