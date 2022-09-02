using Microsoft.AspNetCore.Mvc;

namespace Dental_Clinic_NET.API.Controllers.Helpers
{
    public interface ISearchedController
    {
        public IActionResult Search(string key);
    }
}
