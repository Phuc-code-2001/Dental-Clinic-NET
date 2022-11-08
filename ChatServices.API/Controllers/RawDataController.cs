using ChatServices.API.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChatServices.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RawDataController : ControllerBase
    {
        ServicesManager _servicesManager;

        public RawDataController(ServicesManager servicesManager)
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
            return Ok(dataset);
        }

        [HttpGet]
        public IActionResult GetAllUserBoxChat()
        {
            var dataset = _servicesManager.DbContext.UsersInChatBoxOfReception
                .Include(cb => cb.User)
                .Include(cb => cb.LastMessage)
                .ToArray();
            var datasetDTO = _servicesManager.AutoMapper.Map<UserInChatBoxOfReceptionDTO[]>(dataset);

            return Ok(datasetDTO);
        }

    }
}
