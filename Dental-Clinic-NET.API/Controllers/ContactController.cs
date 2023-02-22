using DataLayer.DataContexts;
using DataLayer.Domain;
using Dental_Clinic_NET.API.DTO;
using Dental_Clinic_NET.API.Models.Contacts;
using Dental_Clinic_NET.API.Services;
using Dental_Clinic_NET.API.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private ServicesManager _servicesManager;

        public ContactController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        /// <summary>
        ///     List all contacts by admin
        /// </summary>
        /// <returns>
        ///     200: Request success
        ///     500: Server Handle Error
        ///     
        /// </returns>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public IActionResult GetAll(int page = 1)
        {
            try
            {
                Paginated<Contact> paginatedContacts = new Paginated<Contact>(_servicesManager.DbContext.Contacts, page);
                var contactDTOs = _servicesManager.AutoMapper.Map<ContactDTO[]>(paginatedContacts.Items.ToArray());

                return Ok(new
                {
                    page,
                    per_page = paginatedContacts.PageSize,
                    total = paginatedContacts.QueryCount,
                    total_pages = paginatedContacts.PageCount,
                    data = contactDTOs
                });

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Create new contact from any actor
        /// </summary>
        /// <param name="request">Contact Info</param>
        /// <returns>
        ///     200: Create success
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        public IActionResult Create(CreateContact request)
        {
            try
            {
                Contact contact = _servicesManager.AutoMapper.Map<Contact>(request);
                _servicesManager.DbContext.Contacts.Add(contact);
                _servicesManager.DbContext.SaveChanges();

                ContactDTO contactDTO = _servicesManager.AutoMapper.Map<ContactDTO>(contact);

                return Ok(contactDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Get a contact details
        /// </summary>
        /// <param name="id">contact id</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Get(int id)
        {
            try
            {
                Contact contact = _servicesManager.DbContext.Contacts.Find(id);
                if(contact == null) return NotFound("Contact not found.");

                ContactDTO contactDTO = _servicesManager.AutoMapper.Map<ContactDTO>(contact);

                return Ok(contactDTO);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Change contact state by admin.
        /// </summary>
        /// <param name="request">id, stateindex</param>
        /// <returns>
        ///     404: Not found contact
        ///     
        /// </returns>
        [HttpPut]
        [Authorize(Roles = "Administrator")]
        public IActionResult ChangeState(UpdateContact request)
        {
            try
            {
                Contact contact = _servicesManager.DbContext.Contacts.Find(request.Id);
                if(contact == null)
                {
                    return NotFound("Contact not found");
                }

                contact.State = request.StateIndex;
                if(contact.State == ContactStates.Done)
                {
                    contact.FinishedTime = DateTime.Now;
                }
                else
                {
                    contact.FinishedTime = null;
                }
                _servicesManager.DbContext.Entry(contact).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                return Ok($"Change state of contact to '{request.StateIndex}' success");
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Remove contact out of database
        /// </summary>
        /// <param name="id">contact id</param>
        /// <returns>
        ///     200: Request success
        ///     500: Server handle error
        /// </returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int id)
        {
            try
            {
                Contact contact = _servicesManager.DbContext.Contacts.Find(id);
                if (contact == null)
                {
                    return NotFound("Contact not found");
                }

                _servicesManager.DbContext.Entry(contact).State = EntityState.Deleted;
                _servicesManager.DbContext.SaveChanges();

                return Ok($"You just have completely delete contact with id='{id}' success");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
