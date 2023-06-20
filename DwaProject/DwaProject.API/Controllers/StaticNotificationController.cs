using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.API.Controllers
{
    [Route("api/static")]
    [Produces("application/json")]
    [ApiController]
    public class StaticNotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public StaticNotificationController(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpGet("[action]")]
        public IActionResult RetreiveUnsentCount()
        {
            try
            {
                int notificationsCount = _notificationRepository.GetUnsentCount();
                var data = new
                {
                    Number = notificationsCount
                };
                return Json(data);
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
        }

        [HttpPost("[action]")]
        public IActionResult SendAll()
        {
            try
            {
                _notificationRepository.SendAll();
                return Ok();
            }
            catch
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "There has been a problem while fetching the data you requested");
            }
        }
    }
}
