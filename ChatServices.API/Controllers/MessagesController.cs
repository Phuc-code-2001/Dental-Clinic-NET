using ChatServices.API.DTOs;
using ChatServices.API.Models;
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
        public IActionResult ListMessagesInConversationOfPatient()
        {
            return Ok();
        }

    }
}
