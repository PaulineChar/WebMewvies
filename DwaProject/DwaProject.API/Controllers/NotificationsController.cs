using DwaProject.BL.BLModels;
using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace DwaProject.API.Controllers
{
	[Route("api/notifications")]
	[ApiController]
	public class NotificationsController : ControllerBase
	{
		private readonly INotificationRepository _notificationRepository;

		public NotificationsController(INotificationRepository notificationRepository)
		{
			_notificationRepository = notificationRepository;
		}

		[HttpGet("[action]")]
		public ActionResult<IEnumerable<BLNotification>> GetAll()
		{
			try
			{
				var notifications = _notificationRepository.GetAll();
				return Ok(notifications);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}


		[HttpGet("{id}")]
		public ActionResult<BLNotification> Get(int id)
		{
			try
			{
				var notification = _notificationRepository.Get(id);
				if (notification == null)
					return NotFound($"Could not find any notification with id {id}");

				return Ok(notification);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPost()]
		public ActionResult<BLNotification> Post(BLNotification notification)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				notification = _notificationRepository.Post(notification);

				return Ok(notification);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPut("{id}")]
		public ActionResult<BLNotification> Post(int id, BLNotification notification)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				notification = _notificationRepository.Post(id, notification);
				if (notification == null)
					return NotFound($"Could not find any notification with id {id}");

				return Ok(notification);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPost("[action]")]
		public ActionResult SendAllNotifications()
		{
			try
			{
				int failed = _notificationRepository.SendAll();
				if(failed > 0)
                    return StatusCode(
                    StatusCodes.Status500InternalServerError, "There has been a problem sending the emails");

                return Ok("All unsent notifications have been sent");
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem sending the emails");
			}
		}

		[HttpDelete("{id}")]
		public ActionResult<BLNotification> Delete(int id)
		{
			try
			{
				var notification = _notificationRepository.Delete(id);
				if (notification == null)
					return NotFound($"Could not find any notification with id {id}");

				return Ok(notification);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}
	}
}
