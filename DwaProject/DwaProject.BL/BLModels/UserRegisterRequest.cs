using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwaProject.BL.BLModels
{
	public class UserRegisterRequest
	{
		[Required, StringLength(50, MinimumLength = 6)]
		public string Username { get; set; }

		[Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

		[Required]
        [DisplayName("Last name")]
        public string LastName { get;set; }

		[Required, StringLength(50, MinimumLength = 8)]
		public string Password { get; set; }

		[Required]
		[EmailAddress]
        [DisplayName("E-mail")]
        public string Email { get; set; }

		[Required]
        [DisplayName("Country of residence")]
        public int CountryOfResidenceId { get; set; }

		[Phone]
		public string? Phone { get; set; }
	}
}
