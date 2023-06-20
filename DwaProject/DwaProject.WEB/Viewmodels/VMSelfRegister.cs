using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DwaProject.WEB.Viewmodels
{
    public class VMSelfRegister
    {
        [Required, DisplayName("User name"), StringLength(50, MinimumLength = 6)]
        public string Username { get; set; }

        [Required, DisplayName("E-mail"), EmailAddress]
        public string Email { get; set; }

        [DisplayName("Confirm e-mail"), EmailAddress]
        [Compare("Email")]
        public string Email2 { get; set; }

        [Required, DisplayName("First name")]
        public string FirstName { get; set; }

        [Required, DisplayName("Last name")]
        public string LastName { get; set; }

        [Required, DisplayName("Password"), StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [DisplayName("Repeat password")]
        [Compare("Password")]
        public string Password2 { get; set; }

        [Required, DisplayName("Country of residence")]
        public int CountryOfResidenceId { get; set; }

        public string? Phone { get; set; }
    }
}
