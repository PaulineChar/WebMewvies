using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Mapping
{
	public static class TagMapper
	{
		public static IEnumerable<Tag> MapToDAL(IEnumerable<BLTag> tags) =>
			tags.Select(x => MapToDAL(x));

		public static Tag MapToDAL(BLTag tag) =>
			new Tag
			{
				Id = tag.Id ?? 0,
				Name = tag.Name,
			};

		public static IEnumerable<BLTag> MapToBL(IEnumerable<Tag> tags) =>
			tags.Select(x => MapToBL(x));

		public static BLTag MapToBL(Tag tag) =>
			new BLTag
			{
				Id = tag.Id,
				Name = tag.Name,
				//Videos = tag.VideoTags.Select(x => x.Video.Name).ToList(),
			};
	}
}
