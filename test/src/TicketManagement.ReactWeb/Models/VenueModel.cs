using System.Collections.Generic;

namespace TicketManagement.ReactWeb.Models
{
    public class VenueModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public string Phone { get; set; }

        public List<LayoutModel> Layouts { get; set; } = new ();
    }
}
