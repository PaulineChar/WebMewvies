using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;

namespace DwaProject.BL.Mapping
{
	public static class GenreMapper
	{
		public static IEnumerable<Genre> MapToDAL(IEnumerable<BLGenre> genres) =>
			genres.Select(x => MapToDAL(x));

		public static Genre MapToDAL(BLGenre genre) =>
			new Genre
			{
				Id = genre.Id ?? 0,
				Name = genre.Name,
				Description = genre.Description,
			};

		public static IEnumerable<BLGenre> MapToBL(IEnumerable<Genre> genres) =>
			genres.Select(x => MapToBL(x));

		public static BLGenre MapToBL(Genre genre) =>
			new BLGenre
			{
				Id = genre.Id,
				Name = genre.Name,
				Description = genre.Description,
				//Videos = genre.Videos.Select(x => x.Name).ToList(),
			};
	}
}
