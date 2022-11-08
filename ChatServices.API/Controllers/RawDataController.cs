using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

    }
}
