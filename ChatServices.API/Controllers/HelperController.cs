using ChatServices.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChatServices.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        ServicesManager _servicesManager;

        public HelperController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        [HttpGet]
        public IActionResult GetRawMessageList()
        {
            var dataset = _servicesManager.DbContext.ChatMessages;

            return Ok(dataset);
        }

        [HttpDelete]
        public IActionResult RemoveAllMessages()
        {
            var dataset = _servicesManager.DbContext.ChatMessages.ToArray();
            _servicesManager.DbContext.ChatMessages.RemoveRange(dataset);
            _servicesManager.DbContext.SaveChanges();
            return Ok(dataset);
        }

        [HttpGet]
        public IActionResult GetAllUserBoxChat()
        {
            var dataset = _servicesManager.DbContext.Conversations
                .Include(cb => cb.User)
                .Include(cb => cb.LastMessage)
                .ToArray();
            var datasetDTO = _servicesManager.AutoMapper.Map<ConversationDTO[]>(dataset);

            return Ok(datasetDTO);
        }

    }
}
