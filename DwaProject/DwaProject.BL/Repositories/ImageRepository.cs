using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Repositories
{
	public interface IImageRepository
	{
		IEnumerable<BLImage> GetAll();
		BLImage Get(int id);

		int Post(BLImage image);

	}
	public class ImageRepository : IImageRepository
	{
		private readonly RwaMoviesContext _dbContext;

		public ImageRepository(RwaMoviesContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<BLImage> GetAll() 
		{
			var dbImages = _dbContext.Images;
			var images = ImageMapper.MapToBL(dbImages);
			return images;
		}

		public BLImage Get(int id)
		{
			var dbImage = _dbContext.Images.FirstOrDefault(i => i.Id == id);
			if (dbImage == null)
				return null;
			
			var image = ImageMapper.MapToBL(dbImage);
			return image;
		}

        public int Post(BLImage image)
        {
            var dbImage = ImageMapper.MapToDAL(image);

			_dbContext.Images.Add(dbImage);
			_dbContext.SaveChanges();

			//The id of the new image, which is necessary to create a video that has this new picture
			return _dbContext.Images.Max(i => i.Id);
        }
    }
}
