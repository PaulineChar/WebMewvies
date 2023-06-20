using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using DwaProject.BL.Mapping;
using System.Net.Mail;
using System.Reflection;

namespace DwaProject.BL.Repositories
{
	public interface INotificationRepository
	{
		IEnumerable<BLNotification> GetAll();
		BLNotification Get(int id);
		BLNotification Post(BLNotification notification);
		BLNotification Post(int id,  BLNotification notification);
		int SendAll();
		int Send(Notification notification);
        int Send(BLNotification notification);

        BLNotification Delete(int id);
        int GetUnsentCount();
    }
	public class NotificationRepository : INotificationRepository
	{
		private readonly RwaMoviesContext _dbContext;

		public NotificationRepository(RwaMoviesContext dbContext)
		{
			_dbContext = dbContext;
		}

		public IEnumerable<BLNotification> GetAll()
		{
			var dbNotifications = _dbContext.Notifications;
			var notifications = NotificationMapper.MapToBL(dbNotifications);
			return notifications;
		}

		public BLNotification Get(int id)
		{
			var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
			if (dbNotification == null)
				return null;

			var notification = NotificationMapper.MapToBL(dbNotification);
			return notification;
		}


		public BLNotification Post(BLNotification notification)
		{
			var dbNotification = NotificationMapper.MapToDAL(notification);

			_dbContext.Notifications.Add(dbNotification);
			_dbContext.SaveChanges();

			return NotificationMapper.MapToBL(dbNotification);
		}

		public int SendAll()
		{
			int failed = 0;

			var dbNotifications = _dbContext.Notifications.Where(n => !n.SentAt.HasValue);
			foreach(var dbNotification in dbNotifications)
			{
				failed += Send(dbNotification);
			}

			return failed;
			
		}

        public int Send(Notification notification)
		{
			//!!!! type smtp4dev in the PMC to start the service
            int failed = 0;
            var client = new SmtpClient("localhost");
            var sender = "admin@mewvies.com";

            var mail = new MailMessage(
                        from: new MailAddress(sender),
                        to: new MailAddress(notification.ReceiverEmail));

            mail.Subject = notification.Subject;
            mail.Body = notification.Body;

			try
			{
                client.Send(mail);

                notification.SentAt = DateTime.UtcNow;

                _dbContext.SaveChanges();
            }
            catch
            {
                failed++;
            }

			return failed;
        }

        public int Send(BLNotification notification)
		{
			var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == (int)notification.Id!);
			return Send(dbNotification);
		}


        public BLNotification Post(int id, BLNotification notification)
		{
			var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
			if (dbNotification == null)
				return null;

			dbNotification.ReceiverEmail = notification.ReceiverEmail;
			dbNotification.Body = notification.Body;
			dbNotification.Subject = notification.Subject;

			_dbContext.SaveChanges();

			return NotificationMapper.MapToBL(dbNotification);
		}

		public BLNotification Delete(int id)
		{
			var dbNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);
			if (dbNotification == null)
				return null;

			_dbContext.Notifications.Remove(dbNotification);

			_dbContext.SaveChanges();

			var notification = NotificationMapper.MapToBL(dbNotification);
			return notification;
		}

        public int GetUnsentCount()
        {
            var count = _dbContext.Notifications.Where(notification => !notification.SentAt.HasValue).Count();
			return count;
        }
    }
}
