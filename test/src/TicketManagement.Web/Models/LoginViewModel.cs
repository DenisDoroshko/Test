using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Web.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password")]
        public string Password { get; set; } = "";

        public string ReturnUrl { get; set; } = "/";
    }
}
