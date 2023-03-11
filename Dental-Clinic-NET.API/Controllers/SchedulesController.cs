using Dental_Clinic_NET.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SchedulesController : ControllerBase
    {
        ServicesManager _servicesManager;

        public SchedulesController(ServicesManager servicesManager)
        {
            _servicesManager = servicesManager;
        }

        

    }
}
