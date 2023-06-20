using DwaProject.BL.BLModels;
using DwaProject.BL.DALModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.Mapping
{
	public static class UserMapper
	{
		public static IEnumerable<User> MapToDAL(IEnumerable<BLUser> users) =>
			users.Select(x => MapToDAL(x));

		public static User MapToDAL(BLUser user) =>
			new User
			{
				Id = user.Id/* ?? 0*/,
				CreatedAt = user.CreatedAt,
				DeletedAt = user.DeletedAt,
				Username = user.Username,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PwdHash = user.PwdHash,
				PwdSalt = user.PwdSalt,
				Phone = user.Phone,
				IsConfirmed = user.IsConfirmed,
				SecurityToken = user.SecurityToken,
				CountryOfResidenceId = user.CountryOfResidenceId,
				CountryOfResidence = user.CountryOfResidence,
			};

		public static IEnumerable<BLUser> MapToBL(IEnumerable<User> users) =>
			users.Select(x => MapToBL(x));

		public static BLUser MapToBL(User user) =>
			new BLUser
			{
				Id = user.Id,
				CreatedAt = user.CreatedAt,
				DeletedAt = user.DeletedAt,
				Username = user.Username,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				PwdHash = user.PwdHash,
				PwdSalt = user.PwdSalt,
				Phone = user.Phone,
				IsConfirmed = user.IsConfirmed,
				SecurityToken = user.SecurityToken,
				CountryOfResidenceId = user.CountryOfResidenceId,
				CountryOfResidence = user.CountryOfResidence,
			};
	}
}
