using AutoMapper;
using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using System.Drawing;

namespace DwaProject.BL.Repositories
{
	
	public interface ITagRepository
	{
		IEnumerable<BLTag> GetAll();
		IEnumerable<BLTag> Search(string namePart);
		(IEnumerable<BLTag>, int) GetAndPage(int page);
		BLTag Get(int id);
		BLTag Post(BLTag tag);
		BLTag Post(int id, BLTag tag);
		BLTag Delete(int id);
	}
	public class TagRepository : ITagRepository
	{
		private readonly RwaMoviesContext _dbContext;

		public TagRepository(RwaMoviesContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<BLTag> GetAll()
		{
			var dbTags = _dbContext.Tags;
			var tags = TagMapper.MapToBL(dbTags);
			return tags;
		}
        public IEnumerable<BLTag> Search(string namePart)
        {
            namePart = namePart.ToLower();
            var dbTags = _dbContext.Tags.Where(x => x.Name.ToLower().Contains(namePart));

            var tags = TagMapper.MapToBL(dbTags);
            return tags;
        }

        public (IEnumerable<BLTag>, int) GetAndPage(int page)
		{
			var dbTags = _dbContext.Tags;
            var tags = TagMapper.MapToBL(dbTags);

            int size = 10;

            var totalPages = tags.Count() / size + (tags.Count() % size == 0 ? 0 : 1);
			tags = tags.Skip((page - 1) * size).Take(size);

            
            return (tags, totalPages);
        }
		public BLTag Get(int id)
		{
			var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
			if (dbTag == null)
				return null;

			var tag = TagMapper.MapToBL(dbTag);
			return tag;
		}


		public BLTag Post(BLTag tag)
		{
			var dbTag = TagMapper.MapToDAL(tag);

			_dbContext.Tags.Add(dbTag);
			_dbContext.SaveChanges();

			return TagMapper.MapToBL(dbTag);
		}

		public BLTag Post(int id, BLTag tag)
		{
			var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
			if (dbTag == null)
				return null;

			dbTag.Name = tag.Name;

			_dbContext.SaveChanges();

			return TagMapper.MapToBL(dbTag);
		}

		public BLTag Delete(int id)
		{
			var dbTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);
			if (dbTag == null)
				return null;

			_dbContext.Tags.Remove(dbTag);

			//Here, if any VideoTag exists, we can't delete them because videos that are still in the database would lose a tag
			//and potentially become tagless

			_dbContext.SaveChanges();

			var tag = TagMapper.MapToBL(dbTag);
			return tag;
		}
	}
	
}
