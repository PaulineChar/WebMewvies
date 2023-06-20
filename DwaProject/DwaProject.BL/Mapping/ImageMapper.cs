using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Mapping
{
	public static class ImageMapper
	{
		public static IEnumerable<Image> MapToDAL(IEnumerable<BLImage> images) =>
			images.Select(x => MapToDAL(x));

		public static Image MapToDAL(BLImage image) =>
			new Image
			{
				Id = image.Id ?? 0,
				Content = image.Content
			};

		public static IEnumerable<BLImage> MapToBL(IEnumerable<Image> images) =>
			images.Select(x => MapToBL(x));

		public static BLImage MapToBL(Image image) =>
			new BLImage
			{
				Id = image.Id,
				Content = image.Content
			};
	}
}
