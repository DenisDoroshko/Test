using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace TicketManagement.Web.Models
{
    public class UserViewModel : IdentityUser
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        public string Surname { get; set; }

        [Required(ErrorMessage = "Time zone is required")]
        public string TimeZoneId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        public override string Email { get; set; }

        public decimal Balance { get; set; }

        [NotMapped]
        public List<string> Roles { get; set; }
    }
}
