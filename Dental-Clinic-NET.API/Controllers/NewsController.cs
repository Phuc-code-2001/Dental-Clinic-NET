using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        ServicesManager _servicesManager { get; set; }

        public NewsController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }



    }
}
