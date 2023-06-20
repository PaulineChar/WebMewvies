using DwaProject.BL.BLModels;
using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.API.Controllers
{
	[Authorize]
	[Route("api/videos")]
	[ApiController]
	public class VideosController : ControllerBase
	{
		private readonly IVideoRepository _videoRepository;
		//private readonly IMapper _mapper;
		public VideosController(IVideoRepository videoRepo)
		{
			_videoRepository = videoRepo;
			//_mapper = mapper;
		}

		[Authorize]
		[HttpGet("[action]")]
		public ActionResult<IEnumerable<BLVideo>> GetAll()
		{
			try
			{
				var allVideos = _videoRepository.GetAll();
				return Ok(allVideos);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[Authorize]
		[HttpGet("[action]")]
		/*
         * name can be null if the user just wants paging and ordering
         */
		public ActionResult<IEnumerable<BLVideo>> Search(string? name, string orderBy, string direction, int page, int size)
		{
			try
			{
				(var videos, _) = _videoRepository.Search(name, orderBy, direction, page, size);

				//return
				return Ok(videos);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[Authorize]
		[HttpGet("{id}")]
		public ActionResult<BLVideo> Get(int id)
		{
			try
			{
				var video = _videoRepository.Get(id);
				if(video == null)
					return NotFound($"Could not find video with id {id}");

				return Ok(video);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[Authorize]
		[HttpPost()]
		public ActionResult<BLVideo> Post(BLVideo video)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				video = _videoRepository.Post(video);

				return Ok(video);

			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}

		}

		[Authorize]
		[HttpPut("{id}")]
		public ActionResult<BLVideo> Post(int id, BLVideo video)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(video);

				video = _videoRepository.Post(id, video);
				if(video == null)
					return NotFound($"Could not find video with id {id}");

				return Ok(video);
			}
			catch (Exception ex)
			{
				return StatusCode(StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[Authorize]
		[HttpDelete("{id}")]
		public ActionResult<BLVideo> Delete(int id)
		{
			try
			{
				var video = _videoRepository.Delete(id);
				
				if (video == null)
					return NotFound($"Could not find video with id {id}");
				

				return Ok(video);
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
