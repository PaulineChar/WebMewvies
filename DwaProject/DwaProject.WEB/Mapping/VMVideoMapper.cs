using DwaProject.BL.BLModels;
using DwaProject.WEB.Viewmodels;

namespace DwaProject.WEB.Mapping
{
	public static class VMVideoMapper
	{
		public static IEnumerable<VMVideo> MapToVM(IEnumerable<BLVideo> videos) =>
			videos.Select(x => MapToVM(x));

		public static VMVideo MapToVM(BLVideo video) =>
			new VMVideo
			{
				Id = video.Id ?? 0,
				Name = video.Name,
				Description = video.Description,
				StreamingUrl = video.StreamingUrl,
				GenreId = video.GenreId,
				ImageId = (int)video.ImageId,
				TotalSeconds = video.TotalSeconds,
				Tags = video.Tags.ToArray()
			};

		public static IEnumerable<BLVideo> MapToBL(IEnumerable<VMVideo> videos) =>
			videos.Select(x => MapToBL(x));

		public static BLVideo MapToBL(VMVideo video) =>
			new BLVideo
			{
				Id = video.Id,
				Name = video.Name,
				Description = video.Description,
				StreamingUrl = video.StreamingUrl,
				GenreId = video.GenreId,
				ImageId = video.ImageId,
				Tags = video.Tags.ToList(),
				TotalSeconds = video.TotalSeconds
			};
	}
}
