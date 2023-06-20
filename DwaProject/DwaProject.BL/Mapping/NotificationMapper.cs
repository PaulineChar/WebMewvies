using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Mapping
{
	public static class NotificationMapper
	{
		public static IEnumerable<Notification> MapToDAL(IEnumerable<BLNotification> notifications) =>
			notifications.Select(x => MapToDAL(x));

		public static Notification MapToDAL(BLNotification notification) =>
			new Notification
			{
				Id = notification.Id ?? 0,
				CreatedAt = notification.CreatedAt,
				ReceiverEmail = notification.ReceiverEmail,
				//SentAt = notification.SentAt,
				Subject = notification.Subject,
				Body = notification.Body,
			};

		public static IEnumerable<BLNotification> MapToBL(IEnumerable<Notification> notifications) =>
			notifications.Select(x => MapToBL(x));

		public static BLNotification MapToBL(Notification notification) =>
			new BLNotification
			{
				Id = notification.Id,
				CreatedAt = notification.CreatedAt,
				ReceiverEmail = notification.ReceiverEmail,
				SentAt = notification.SentAt,
				Subject = notification.Subject,
				Body = notification.Body,
			};
	}
}
