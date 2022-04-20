using System.Collections.Generic;

namespace TicketManagement.ReactWeb.Models
{
    public class UserModel
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string TimeZoneId { get; set; }

        public string Email { get; set; }

        public decimal Balance { get; set; }

        public List<string> Roles { get; set; }
    }
}
