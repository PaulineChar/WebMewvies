using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.API.Controllers
{
	[Route("api/tags")]
	[ApiController]
	public class TagsController : ControllerBase
	{
		private readonly ITagRepository _tagRepository;
		public TagsController(ITagRepository tagRepository)
		{
			_tagRepository = tagRepository;
		}

		[HttpGet("[action]")]
		public ActionResult<IEnumerable<BLTag>> GetAll()
		{
			try
			{
				var tags = _tagRepository.GetAll();
				return Ok(tags);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}

		}

		[HttpGet("[action]")]
		public ActionResult<IEnumerable<BLTag>> Search(string namePart)
		{
			try
			{
				var tags = _tagRepository.Search(namePart);
				return Ok(tags);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpGet("{id}")]
		public ActionResult<BLTag> Get(int id)
		{
			try
			{
				var tag = _tagRepository.Get(id);
				if (tag == null)
				{
					return NotFound($"Could not find any tag with id {id}");
				}

				return Ok(tag);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPost()]
		public ActionResult<BLTag> Post(BLTag tag)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				tag = _tagRepository.Post(tag);

				return Ok(tag);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPut("{id}")]
		public ActionResult<BLTag> Post(int id, BLTag tag)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				tag = _tagRepository.Post(id, tag);
				if (tag == null)
					return NotFound($"Could not find any tag with id {id}");

				return Ok(tag);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpDelete("{id}")]
		public ActionResult<BLTag> Delete(int id)
		{
			try
			{
				var tag =_tagRepository.Delete(id);
				if (tag == null)
					return NotFound($"Could not find any tag with id {id}");


				return Ok(tag);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
                    "This tag is linked to at least one movie and, therefore, cannot be deleted");
			}
		}
	
	}
}
