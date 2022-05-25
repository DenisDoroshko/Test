using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.UserApi.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Username")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Password")]
        public string Password { get; set; } = "";
    }
}
