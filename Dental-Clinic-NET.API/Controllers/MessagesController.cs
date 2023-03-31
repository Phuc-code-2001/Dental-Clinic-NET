using DataLayer.Domain;
using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System;
using Dental_Clinic_NET.API.Models.Chats;
using Dental_Clinic_NET.API.DTOs.Messages;
using Dental_Clinic_NET.API.Utils;

namespace Dental_Clinic_NET.API.Controllers
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
        ///     Send a message from Patient to Receptionist (any Receptionist can view and reply)
        /// </summary>
        /// <param name="request">Include Content</param>
        /// <returns>
        ///     200: Request success
        ///     401: Unauthorize
        ///     403: Forbiden
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = nameof(UserType.Patient))]
        public IActionResult PatToRec(PatToRecMessage request)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                Message message = _servicesManager.AutoMapper.Map<Message>(request);
                message.FromId = loggedUser.Id;

                var conversation = _servicesManager.DbContext.Conversations
                    .FirstOrDefault(cb => cb.UserId == loggedUser.Id);

                if (conversation != null)
                {
                    conversation.LastMessage = message;
                    conversation.HasMessageUnRead = true;

                    _servicesManager.DbContext.Entry(conversation).State = EntityState.Modified;
                }
                else
                {
                    conversation = new Conversation()
                    {
                        UserId = message.FromId,
                        HasMessageUnRead = true,
                        LastMessage = message,
                    };

                    _servicesManager.DbContext.Conversations.Add(conversation);
                }

                _servicesManager.DbContext.Add(message);
                _servicesManager.DbContext.SaveChanges();

                ChatMessageDTO messageDTO = _servicesManager.AutoMapper.Map<ChatMessageDTO>(message);

                // Push event
                string[] chanels = _servicesManager.DbContext.Users
                    .Where(user => user.Type == UserType.Receptionist)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushToAsync(chanels, "Chat-PatToRec", messageDTO, result =>
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

        /// <summary>
        /// Send a message from Receptionist to Patient
        /// </summary>
        /// <param name="request">Include patientId, and Content</param>
        /// <returns>
        ///     200: Request success
        ///     404: Patient not found
        ///     401: Unauthorize
        ///     403: Forbiden
        ///     500: Server handle error
        /// </returns>
        [HttpPost]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult RecToPat(RecToPatMessage request)
        {
            try
            {
                Patient toPatient = _servicesManager.DbContext.Patients
                    .Include(pat => pat.BaseUser)
                    .FirstOrDefault(pat => pat.Id == request.PatientId);

                if (toPatient == null)
                {
                    return NotFound($"Patient not found with id= '{request.PatientId}'!");
                }

                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                Message message = _servicesManager.AutoMapper.Map<Message>(request);
                message.FromId = loggedUser.Id;

                Conversation conversation = _servicesManager.DbContext.Conversations
                    .FirstOrDefault(cb => cb.UserId == toPatient.Id);

                if (conversation != null)
                {
                    conversation.LastMessage = message;
                    conversation.HasMessageUnRead = false;
                    _servicesManager.DbContext.Entry(conversation).State = EntityState.Modified;
                }
                else
                {
                    conversation = new Conversation()
                    {
                        UserId = message.ToId,
                        HasMessageUnRead = false,
                        LastMessage = message,
                    };

                    _servicesManager.DbContext.Conversations.Add(conversation);
                }

                _servicesManager.DbContext.Add(message);
                _servicesManager.DbContext.SaveChanges();

                ChatMessageDTO messageDTO = _servicesManager.AutoMapper.Map<ChatMessageDTO>(message);

                // Push event
                string[] chanels = _servicesManager.DbContext.Users
                    .Where(user => user.Id == toPatient.Id)
                    .Select(user => user.PusherChannel).ToArray();

                Task pushEventTask = _servicesManager.PusherServices
                    .PushToAsync(chanels, "Chat-RecToPat", messageDTO, result =>
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

        /// <summary>
        ///     List User ChatBox by Reception
        /// </summary>
        /// <returns>
        ///     200: Request success
        ///     401: Unauthorize
        ///     403: Forbiden
        ///     500: Server handle error
        /// </returns>
        [HttpGet]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult ListUsersChatBox()
        {
            try
            {

                // 1. Tạo thêm bảng lưu trữ những hộp thoại đang mở
                // 2. Khi có người gửi tin nhắn, căn cứ vào hộp thoại có tồn tại không mà thêm vào database
                // 3. Truy vấn vào hộp thoại khi muốn hiện danh sách

                var dataset = _servicesManager.DbContext.Conversations
                .Include(cb => cb.User)
                .Include(cb => cb.LastMessage)
                .ToArray();
                var datasetDTO = _servicesManager.AutoMapper.Map<ConversationDTO[]>(dataset);

                return Ok(datasetDTO.OrderBy(x => x.Seen).ThenByDescending(x => x.LastMessageCreated));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     List messages in chatbox of a patient
        /// </summary>
        /// <param name="page"></param>
        /// <returns>
        ///     200: Request success
        ///     401: Unauthorize
        ///     403: Forbiden
        ///     500: Server handle error
        /// </returns>
        [HttpGet]
        [Authorize(Roles = nameof(UserType.Patient))]
        public IActionResult ListMessagesInConversationOfPatient([FromQuery] PageFilter filter)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);

                var queries = _servicesManager.DbContext.ChatMessages
                    .Include(message => message.FromUser)
                    .Where(message => message.FromUser.Id == loggedUser.Id || message.ToUser.Id == loggedUser.Id)
                    .OrderByDescending(message => message.TimeCreated);

                var paginated = new Paginated<Message>(queries, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<ChatMessageDTO[]>(items.ToArray()));

                return Ok(dataset);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     List messages in chatbox of a reception with a patient
        /// </summary>
        /// <param name="page"></param>
        /// <returns>
        ///     200: Request success
        ///     404: Patient not found
        ///     401: Unauthorize
        ///     403: Forbiden
        ///     500: Server handle error
        /// </returns>
        [HttpGet("{patientId}")]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult ListMessagesInConversationOfReception(string patientId, [FromQuery] PageFilter filter)
        {
            try
            {
                Patient toPatient = _servicesManager.DbContext.Patients
                    .Include(pat => pat.BaseUser)
                    .FirstOrDefault(pat => pat.Id == patientId);

                if (toPatient == null)
                {
                    return NotFound($"Patient not found with id='{patientId}'!");
                }

                var queries = _servicesManager.DbContext.ChatMessages
                    .Include(message => message.FromUser)
                    .Include(message => message.ToUser)
                    .Where(message => message.FromId == patientId || message.ToId == patientId)
                    .OrderByDescending(message => message.TimeCreated);

                var paginated = new Paginated<Message>(queries, filter.Page, filter.PageSize);

                var dataset = paginated.GetData(items => _servicesManager.AutoMapper.Map<ChatMessageDTO[]>(items.ToArray()));

                return Ok(dataset);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        ///     Mark seen a chat box with user
        /// </summary>
        /// <param name="chatBoxId">Chatbox Id</param>
        /// <returns>
        ///     404: ChatBox not found
        ///     401: Unauthorize
        ///     403: Not allowed
        ///     500: Server handle error
        /// </returns>
        [HttpPut("{chatBoxId}")]
        [Authorize(Roles = nameof(UserType.Receptionist))]
        public IActionResult MarkAsSeenChatBox(int chatBoxId)
        {
            try
            {
                var userChatBox = _servicesManager.DbContext
                    .Conversations.Include(cb => cb.User)
                    .FirstOrDefault(cb => cb.Id == chatBoxId);

                if (userChatBox == null)
                {
                    return NotFound($"ChatBox {chatBoxId} not found!");
                }

                userChatBox.HasMessageUnRead = false;
                _servicesManager.DbContext.Entry(userChatBox).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                // Push event
                string[] chanels = new string[]
                {
                    userChatBox.User.PusherChannel
                };

                string message = "Reception just have been seen your messages.";
                Task pushEventTask = _servicesManager.PusherServices
                    .PushToAsync(chanels, "Chat-MarkAsSeenChatBox", message, result =>
                    {
                        Console.WriteLine("Chat-MarkAsSeenChatBox Done at: " + DateTime.Now);
                        Console.WriteLine("Data: " + message);
                        Console.WriteLine("To Chanels: ");
                        foreach (string chanel in chanels)
                        {
                            Console.WriteLine(chanel);
                        }
                    });

                return Ok($"Just be seen chatbox {userChatBox.Id}.");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{messageId}")]
        [Authorize]
        public IActionResult RemoveMessage(int messageId)
        {
            try
            {
                BaseUser loggedUser = _servicesManager.UserServices.GetLoggedUser(HttpContext);


                Message message = _servicesManager.DbContext.ChatMessages
                    .FirstOrDefault(message => message.Id == messageId);

                if (message == null || message.FromId != loggedUser.Id)
                {
                    // Hide messageId from hacker
                    return StatusCode(403, "Cannot do operation!");
                }

                message.IsRemoved = true;
                _servicesManager.DbContext.Entry(message).State = EntityState.Modified;
                _servicesManager.DbContext.SaveChanges();

                return Ok($"Bạn đã thu hồi tin nhắn. Mã: {messageId}!");

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
