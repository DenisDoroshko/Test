using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TicketManagement.UserApi.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Username")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Name")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Surname")]
        public string Surname { get; set; } = "";

        [Required(ErrorMessage = "Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{6,}$", ErrorMessage = "PasswordExpression")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "RepeatPassword")]
        [Compare("Password", ErrorMessage = "RepeatPasswordCompare")]
        public string RepeatPassword { get; set; } = "";

        [Required(ErrorMessage = "Email")]
        [EmailAddress(ErrorMessage = "EmailExpression")]
        public string Email { get; set; } = "";

        public string Role { get; set; } = Roles.User;
    }
}
