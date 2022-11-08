using ChatServices.API.DTOs;
using ChatServices.API.Models;
using ChatServices.API.Utils;
using DataLayer.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatServices.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        ServicesManager _servicesManager;

        public MessagesController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        /// <summary>
        ///     Send a message from patient to receptionist (receptionist auto detect)
        /// </summary>
        /// <returns>
        /// 
        /// </returns>
        [HttpPost]
        [Authorize(Roles = nameof(UserType.Patient))]
        public IActionResult PatToRec(PatToRecMessage request)
        {
            try
            {
                string loggedUserName = User.Identity.Name;
                BaseUser loggedUser = _servicesManager.DbContext
                    .Users.FirstOrDefault(u => u.UserName == loggedUserName);

                ChatMessage message = _servicesManager.AutoMapper.Map<ChatMessage>(request);
                message.FromId = loggedUser.Id;

                _servicesManager.DbContext.Add(message);
                _servicesManager.DbContext.SaveChanges();

                ChatMessageDTO messageDTO = _servicesManager.AutoMapper.Map<ChatMessageDTO>(message);

                // Push event
                string[] chanels = _servicesManager.DbContext.Users
                    .Where(user => user.Type == UserType.Receptionist)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Chat-PatToRec", messageDTO, result =>
                    {
                        Console.WriteLine("Chat-PatToRec Done at: " + DateTime.Now);
                        Console.WriteLine("Data: " + JsonConvert.SerializeObject(messageDTO, Formatting.Indented));
                        Console.WriteLine("To Chanels: ");
                        foreach(string chanel in chanels)
                        {
                            Console.WriteLine(chanel);
                        }
                    });

                return Ok(messageDTO);

            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpPost]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult RecToPat(RecToPatMessage request)
        {
            try
            {
                Patient toPatient = _servicesManager.DbContext.Patients
                    .Include(pat => pat.BaseUser)
                    .FirstOrDefault(pat => pat.Id == request.PatientId);

                if(toPatient == null)
                {
                    return BadRequest("Truyền cái PatientId đúng chưa? Patient not found!");
                }

                string loggedUserName = User.Identity.Name;
                BaseUser loggedUser = _servicesManager.DbContext
                    .Users.FirstOrDefault(u => u.UserName == loggedUserName);

                ChatMessage message = _servicesManager.AutoMapper.Map<ChatMessage>(request);
                message.FromId = loggedUser.Id;

                message.ToId = request.PatientId;

                _servicesManager.DbContext.Add(message);
                _servicesManager.DbContext.SaveChanges();

                ChatMessageDTO messageDTO = _servicesManager.AutoMapper.Map<ChatMessageDTO>(message);

                // Push event
                string[] chanels = _servicesManager.DbContext.Users
                    .Where(user => user.Id == toPatient.Id)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushTo(chanels, "Chat-RecToPat", messageDTO, result =>
                    {
                        Console.WriteLine("Chat-PatToRec Done at: " + DateTime.Now);
                        Console.WriteLine("Data: " + JsonConvert.SerializeObject(messageDTO, Formatting.Indented));
                        Console.WriteLine("To Chanels: ");
                        foreach (string chanel in chanels)
                        {
                            Console.WriteLine(chanel);
                        }
                    });

                return Ok(messageDTO);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        [HttpGet]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult ListUsersHasMessage()
        {
            try
            {

                var queries = _servicesManager.DbContext.ChatMessages
                    .Include(message => message.FromUser)
                    .Where(message => message.FromUser.Type == UserType.Patient);

                var userGroup = queries.AsEnumerable().GroupBy(message => message.FromUser)
                    .Select(group => new
                    {
                        User = _servicesManager.AutoMapper.Map<ChatUserDTO>(group.Key),
                        HasMessageUnRead = group.Any(message => !message.IsRead),
                        LastMessageCreated = group.Max(message => message.TimeCreated)
                    });

                // Hơi tốn performance, fix sau

                return Ok(userGroup);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Authorize(Roles = nameof(UserType.Patient))]
        public IActionResult ListMessagesInConversationOfPatient(int page = 1)
        {
            try
            {
                string loggedUserName = User.Identity.Name;
                BaseUser loggedUser = _servicesManager.DbContext
                    .Users.FirstOrDefault(u => u.UserName == loggedUserName);

                var queries = _servicesManager.DbContext.ChatMessages
                    .Include(message => message.FromUser)
                    .Where(message => message.FromUser.Id == loggedUser.Id || message.ToUser.Id == loggedUser.Id)
                    .OrderByDescending(message => message.TimeCreated);

                var paginated = new Paginated<ChatMessage>(queries, page);

                ChatMessageDTO[] datasetDTO = _servicesManager.AutoMapper
                    .Map<ChatMessageDTO[]>(paginated.Items.ToArray());

                return Ok(new
                {
                    page = page,
                    per_page = paginated.PageSize,
                    total = paginated.QueryCount,
                    total_pages = paginated.PageCount,
                    data = datasetDTO,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{patientId}")]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult ListMessagesInConversationOfReception(string patientId, int page = 1)
        {
            try
            {
                
                var queries = _servicesManager.DbContext.ChatMessages
                    .Include(message => message.FromUser)
                    .Where(message => message.FromUser.Id == patientId || message.ToUser.Id == patientId)
                    .OrderByDescending(message => message.TimeCreated);

                var paginated = new Paginated<ChatMessage>(queries, page);

                ChatMessageDTO[] datasetDTO = _servicesManager.AutoMapper
                    .Map<ChatMessageDTO[]>(paginated.Items.ToArray());

                return Ok(new
                {
                    page = page,
                    per_page = paginated.PageSize,
                    total = paginated.QueryCount,
                    total_pages = paginated.PageCount,
                    data = datasetDTO,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

    }
}
