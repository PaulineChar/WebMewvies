using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DwaProject.BL.Repositories
{
	public interface IVideoRepository
	{
		IEnumerable<BLVideo> GetAll();
		(IEnumerable<BLVideo>, int) Search(string? name, string orderBy, string direction, int page, int size);
		BLVideo Get(int id);
		BLVideo Post(BLVideo video);
		BLVideo Post(int id, BLVideo video);
		BLVideo Delete(int id);
		(IEnumerable<BLVideo>, int) SearchByGenreName(string? name, int page, int size);

    }
	public class VideoRepository : IVideoRepository
	{
		private readonly RwaMoviesContext _dbContext;
		private readonly IGenreRepository _genreRepository;
		public VideoRepository(RwaMoviesContext dbContext, IGenreRepository genreRepository) 
		{
			_dbContext = dbContext;
			_genreRepository = genreRepository;
		}

		public IEnumerable<BLVideo> GetAll()
		{
			var dbVideos = _dbContext.Videos.Include("VideoTags").Include("VideoTags.Tag");
			//var allVideos = _mapper.Map<List<Video>>(_dbContext.Videos);
			var allVideos = VideoMapper.MapToBL(dbVideos);
			return allVideos;
		}

		public (IEnumerable<BLVideo>, int) Search(string? name, string orderBy, string direction, int page, int size)
		{
			//get the data
			var dbVideos = _dbContext.Videos.Include("VideoTags").Include("VideoTags.Tag");
			var videos = VideoMapper.MapToBL(dbVideos);
			//IEnumerable<Video> videos = _mapper.Map<List<Video>>(_dbContext.Videos);

			//filter by name (or part of name) if given
			if (name != null)
				videos = videos.Where(v => v.Name.ToLower().Contains(name.ToLower()));

			//Ordering
			if (string.Compare(orderBy, "id", true) == 0)
			{
				videos = videos.OrderBy(v => v.Id);
			}
			else if (string.Compare(orderBy, "name", true) == 0)
			{
				videos = videos.OrderBy(v => v.Name);
			}
			else if (string.Compare(orderBy, "totaltime", true) == 0)
			{
				videos = videos.OrderBy(v => v.TotalSeconds);
			}
			else //by default, order by id
			{
				videos = videos.OrderBy(v => v.Id);
			}

			//if descending order
			if (string.Compare(direction, "desc", true) == 0)
				videos = videos.Reverse();

			int count = videos.Count();

            int totalPages = count/size + (count%size == 0 ? 0 : 1);

			//Paging
			//First page is 1
			videos =
				videos.Skip((page - 1) * size)
				.Take(size);

			return (videos, totalPages);
		}

		public (IEnumerable<BLVideo>, int) SearchByGenreName(string name, int page, int size)
		{
			//get the data
			var dbVideos = _dbContext.Videos.Include("VideoTags").Include("VideoTags.Tag").Include("Genre");
			var dbGenres = _genreRepository.Search(name);
			List<int> genreIds = new List<int>();

			foreach (var genre in dbGenres)
				genreIds.Add((int)genre.Id!);

			dbVideos = dbVideos
				.Where(v => genreIds.Contains(v.GenreId));

			int count = dbVideos.Count();
			int totalPages = count / size + (count % size == 0 ? 0 : 1);

			dbVideos = dbVideos.Skip((page - 1) * size)
							   .Take(size);

            var videos = VideoMapper.MapToBL(dbVideos);

            return (videos, totalPages);
        }


        public BLVideo Get(int id)
		{
			var dbVideos = _dbContext.Videos.Include("VideoTags").Include("VideoTags.Tag");
			var dbVideo = dbVideos.FirstOrDefault(v => v.Id == id);

			if (dbVideo == null)
				return null;

			var video = VideoMapper.MapToBL(dbVideo);
			
			return video;
		}

		public BLVideo Post(BLVideo video)
		{
			var dbVideo = VideoMapper.MapToDAL(video);

			//Get the tags in their database form
			var dbTags = _dbContext.Tags.Where(x => video.Tags.Contains(x.Name));
			dbVideo.VideoTags = dbTags.Select(x => new VideoTag { Tag = x }).ToList();


			_dbContext.Videos.Add(dbVideo);
			_dbContext.SaveChanges();

			video = VideoMapper.MapToBL(dbVideo);

			return video;
		}

		public BLVideo Post(int id, BLVideo video)
		{
			var dbVideo = _dbContext.Videos.Include("VideoTags").Include("VideoTags.Tag").FirstOrDefault(v => v.Id == id);
			if (dbVideo == null)
				return null;


			dbVideo.Name = video.Name;
			dbVideo.Description = video.Description;
			dbVideo.StreamingUrl = video.StreamingUrl;
			dbVideo.ImageId = video.ImageId;
			dbVideo.GenreId = video.GenreId;
			dbVideo.TotalSeconds = video.TotalSeconds;

			// Remove unused tags
			var toRemove = dbVideo.VideoTags.Where(x => !video.Tags.Contains(x.Tag.Name));
			foreach (var srTag in toRemove)
			{
				_dbContext.VideoTags.Remove(srTag);
			}

			// Add new tags
			var existingDbTagNames = dbVideo.VideoTags.Select(x => x.Tag.Name);
			var newTagNames = video.Tags.Except(existingDbTagNames);
			foreach (var newTagName in newTagNames)
			{
				var dbTag = _dbContext.Tags.FirstOrDefault(x => newTagName == x.Name);
				// What if the tag doesn't exist at all?
				if (dbTag == null)
					continue;

				dbVideo.VideoTags.Add(new VideoTag
				{
					Video = dbVideo,
					Tag = dbTag
				});
			}

			_dbContext.SaveChanges();

			video = VideoMapper.MapToBL(dbVideo);

			return video;
		}

		public BLVideo Delete(int id)
		{
			var dbVideo = _dbContext.Videos.Include("VideoTags").Include("VideoTags.Tag").FirstOrDefault(x => x.Id == id);
			if (dbVideo == null)
				return null;

			var video = VideoMapper.MapToBL(dbVideo);


			//We have to remove the VideoTag to "break" the link between the Video and its tags
			var dbVideoTags = _dbContext.VideoTags.Where(y => y.VideoId == id);
			foreach (var dbTag in dbVideoTags)
			{
				_dbContext.VideoTags.Remove(dbTag);
			}

			_dbContext.Videos.Remove(dbVideo);

			

			_dbContext.SaveChanges();

			return video;
		}
	}
}
