using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.Controllers.Helpers
{
    public interface IPaginatedController
    {
        public IActionResult GetPage(int? pageIndex);
    }
}
