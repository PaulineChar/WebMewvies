using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DwaProject.WEB.Viewmodels
{
	public class VMVideo
	{
		public int? Id { get; set; }


		public string Name { get; set; } = null!;

		public string? Description { get; set; }

		[DisplayName("Genre Id")]
		public int GenreId { get; set; }

        [DisplayName("Genre")]
        public string? GenreName { get; set; }

        [DisplayName("Duration (s)")]
        public int TotalSeconds { get; set; }

        [DisplayName("Streaming Url")]
		[Url]
        public string StreamingUrl { get; set; }

        [DisplayName("Image Id")]
        public int ImageId { get; set; }

		[DisplayName("Image")]
		public string ImageContent { get; set; }

		public string[] Tags { get; set; }

        public IFormFile? ImageFile { get; set; }
    }
}
