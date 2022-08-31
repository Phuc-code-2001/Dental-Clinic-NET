using DataLayer.Schemas;
using DataLayer.Services.GroupMembers;
using Dental_Clinic_NET.API.Models.GroupMemberModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupMemberController : ControllerBase
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
            return Ok(_groupMemberServices.GetById(id));
        }

        [HttpPost]
        public IActionResult Insert(CreateGroupMemberModel createModel)
        {
            try
            {
                int id = _groupMemberServices.Insert(createModel);
                return GetById(id);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }

        [HttpPut]
        public IActionResult Update()
        {
            return Ok("not implement...");
        }

        [HttpDelete]
        public IActionResult Remove()
        {
            return Ok("not implement...");
        }


        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_groupMemberServices.GetAll());
        }

    }
}
