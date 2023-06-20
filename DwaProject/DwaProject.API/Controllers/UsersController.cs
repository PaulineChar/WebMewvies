using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Web;

namespace DwaProject.API.Controllers
{
	[Route("api/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly INotificationRepository _notificationRepository;
		private const string SUBJECT = "Validate your email";
		private const string LINK = "https://localhost:7291/api/users/ValidateEmail/";
		private const string BODY = "Validate your email by pressing this link: ";

		public UsersController(IUserRepository userRepository, INotificationRepository notificationRepository)
		{
			_userRepository = userRepository;
			_notificationRepository = notificationRepository;
		}


		[HttpPost("[action]")]
		public ActionResult<BLUser> Register([FromBody] UserRegisterRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);
			try
			{
				var newUser = _userRepository.Register(request);

                //After the user is registered, we have to create a notification
                BLNotification notification = new BLNotification
				{
					CreatedAt = DateTime.UtcNow,
					ReceiverEmail = newUser.Email,
					Subject = SUBJECT,
					Body = BODY + LINK + HttpUtility.UrlEncode(newUser.Username) + "/" + HttpUtility.UrlEncode(newUser.SecurityToken)
				};


				_notificationRepository.Post(notification);

				return Ok(new UserRegisterResponse
				{
					Id = newUser.Id,
					SecurityToken = newUser.SecurityToken
				});
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}


		[HttpGet("[action]/{username}/{b64SecToken}")]
		public ActionResult ValidateEmail(string username, string b64SecToken)
		{
			try
			{
                ValidateEmailRequest request = new ValidateEmailRequest() { Username = username, B64SecToken = b64SecToken };

                _userRepository.ValidateEmail(request);
				return Redirect("https://localhost:7291/validation.html");
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/*[HttpGet("[action]/{username}/{b64SecToken}")]
		public ActionResult Validate(string username, string b64SecToken)
		{

			return ValidateEmail(new ValidateEmailRequest { Username = HttpUtility.UrlDecode(username), B64SecToken = HttpUtility.UrlDecode(b64SecToken) });
		}*/

		[HttpPost("[action]")]
		public ActionResult<Tokens> JwtTokens([FromBody] JwtTokensRequest request)
		{
			try
			{
				return Ok(_userRepository.JwtTokens(request));
			}
			catch (InvalidOperationException ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}
}
