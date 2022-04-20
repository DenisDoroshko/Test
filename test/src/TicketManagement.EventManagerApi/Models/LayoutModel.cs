using System.Collections.Generic;

namespace TicketManagement.EventManagerApi.Models
{
    public class LayoutModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int VenueId { get; set; }

        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<AreaModel> Areas { get; set; } = new ();
    }
}
