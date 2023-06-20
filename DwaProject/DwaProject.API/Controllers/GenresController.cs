using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DwaProject.API.Controllers
{
	[Route("api/genres")]
	[ApiController]
	public class GenresController : ControllerBase
	{
		private readonly IGenreRepository _genreRepository;

		public GenresController(IGenreRepository genreRepository)
		{
			_genreRepository = genreRepository;
		}

		[HttpGet("[action]")]
		public ActionResult<IEnumerable<BLGenre>> GetAll()
		{
			try
			{
				var genres = _genreRepository.GetAll();
				return Ok(genres);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpGet("[action]")]
		public ActionResult<IEnumerable<BLGenre>> Search(string namePart)
		{
			try
			{
				var genres = _genreRepository.Search(namePart);
				return Ok(genres);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpGet("{id}")]
		public ActionResult<BLGenre> Get(int id)
		{
			try
			{
				var genre = _genreRepository.Get(id);
				if(genre == null)
					return NotFound($"Could not find any genre with id {id}");

				return Ok(genre);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPost()]
		public ActionResult<BLGenre> Post(BLGenre genre)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				genre = _genreRepository.Post(genre);

				return Ok(genre);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpPut("{id}")]
		public ActionResult<BLGenre> Post(int id, BLGenre genre)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);

				genre = _genreRepository.Post(id, genre);
				if(genre == null)
					return NotFound($"Could not find any genre with id {id}");

				return Ok(genre);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"There has been a problem while fetching the data you requested");
			}
		}

		[HttpDelete("{id}")]
		public ActionResult<BLGenre> Delete(int id)
		{
			try
			{
				var genre = _genreRepository.Delete(id);
				if(genre == null)
					return NotFound($"Could not find any genre with id {id}");

				return Ok(genre);
			}
			catch (Exception ex)
			{
				return StatusCode(
					StatusCodes.Status500InternalServerError,
					"This genre is linked to at least one movie and, therefore, cannot be deleted");
			}
		}
	
	}
}
