using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;

namespace DwaProject.BL.Repositories
{
	public interface IGenreRepository
	{
		IEnumerable<BLGenre> GetAll();
		IEnumerable<BLGenre> Search(string namePart);
		(IEnumerable<BLGenre>, int) SearchAndPage(string namePart, int page);

        BLGenre Get(int id);
		BLGenre Post(BLGenre genre);
		BLGenre Post(int id, BLGenre genre);
		BLGenre Delete(int id);
	}
	public class GenreRepository : IGenreRepository
	{
		private readonly RwaMoviesContext _dbContext;

		public GenreRepository(RwaMoviesContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<BLGenre> GetAll()
		{
			var dbGenres = _dbContext.Genres;
			var genres = GenreMapper.MapToBL(dbGenres);
			return genres;
		}
		public IEnumerable<BLGenre> Search(string namePart)
		{
			var dbGenres = _dbContext.Genres.Where(x => x.Name.ToLower().Contains(namePart.ToLower()));
			var genres = GenreMapper.MapToBL(dbGenres);
			return genres;
		}

        public (IEnumerable<BLGenre>, int) SearchAndPage(string namePart, int page)
        {
			var dbGenres = _dbContext.Genres.Where(g => g.Name.ToLower().Contains(namePart.ToLower()));
			int size = 10;
			int count = dbGenres.Count();
			int totalPages = count / size + (count % size == 0 ? 0 : 1);

			dbGenres = dbGenres.Skip((page - 1) * size).Take(size);
			var blGenres = GenreMapper.MapToBL(dbGenres);

			return (blGenres, totalPages);
        }

        public BLGenre Get(int id)
		{
			var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
			if (dbGenre == null)
				return null;

			var genre = GenreMapper.MapToBL(dbGenre);
			return genre;
		}


		public BLGenre Post(BLGenre genre)
		{
			var dbGenre = GenreMapper.MapToDAL(genre);

			_dbContext.Genres.Add(dbGenre);
			_dbContext.SaveChanges();

			return GenreMapper.MapToBL(dbGenre);
		}

		public BLGenre Post(int id, BLGenre genre)
		{
			var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
			if (dbGenre == null)
				return null;

			dbGenre.Name = genre.Name;
			dbGenre.Description = genre.Description;

			_dbContext.SaveChanges();

			return GenreMapper.MapToBL(dbGenre);
		}

		public BLGenre Delete(int id)
		{
			var dbGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);
			if (dbGenre == null)
				return null;

			_dbContext.Genres.Remove(dbGenre);

			_dbContext.SaveChanges();

			var genre = GenreMapper.MapToBL(dbGenre);
			return genre;
		}

		
	}
}
