using System.Collections.Generic;

namespace TicketManagement.Web.Models
{
    public class EventAreaViewModel
    {
        public int Id { get; set; }

        public int EventId { get; set; }

        public string Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public string Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public decimal Price { get; set; }

        public List<EventSeatViewModel> EventSeats { get; set; }
    }
}
