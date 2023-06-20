using System.ComponentModel.DataAnnotations;

namespace DwaProject.BL.BLModels
{
	public class BLNotification
	{
		public int? Id { get; set; }

        public DateTime CreatedAt { get; set; }

		public DateTime? SentAt { get; set; }

        [EmailAddress]
		public string ReceiverEmail { get; set; } = null!;

		public string? Subject { get; set; }

		public string Body { get; set; } = null!;

	}
}
