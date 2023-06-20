using Microsoft.AspNetCore.Mvc;

namespace DwaProject.WEB.Controllers.Shared
{
    public class StatusCodeController : Controller
    {
        [HttpGet("/StatusCode/{statusCode}")]
        public IActionResult Index(int statusCode)
        {
            if(statusCode == 404)
            {
                return View("404");
            }
            return View("Default");
        }
    }
}
