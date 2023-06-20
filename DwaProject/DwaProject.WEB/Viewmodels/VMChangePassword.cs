using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DwaProject.WEB.Viewmodels
{
    public class VMChangePassword
    {
        [Required, DisplayName("User name")]
        public string Username { get; set; }

        [Required, DisplayName("Password"), StringLength(50, MinimumLength = 8)]
        public string Password { get; set; }

        [Required, DisplayName("New Password"), StringLength(50, MinimumLength = 8)]
        public string NewPassword { get; set; }
    }
}
