using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Mapping
{
	public static class VideoMapper
	{
		public static IEnumerable<Video> MapToDAL(IEnumerable<BLVideo> videos) =>
			videos.Select(x => MapToDAL(x));

		public static Video MapToDAL(BLVideo video) =>
			new Video
			{
				Id = video.Id ?? 0,
				Name = video.Name,
				Description = video.Description,
				StreamingUrl = video.StreamingUrl,
				GenreId = video.GenreId,
				ImageId = video.ImageId,
				TotalSeconds = video.TotalSeconds
			};

		public static IEnumerable<BLVideo> MapToBL(IEnumerable<Video> videos) =>
			videos.Select(x => MapToBL(x));

		public static BLVideo MapToBL(Video video) =>
			new BLVideo
			{
				Id = video.Id,
				Name = video.Name,
				Description = video.Description,
				StreamingUrl = video.StreamingUrl,
				GenreId = video.GenreId,
				ImageId = video.ImageId,
				Tags = video.VideoTags.Select(x => x.Tag.Name).ToList(),
				TotalSeconds = video.TotalSeconds
			};
	}
}
