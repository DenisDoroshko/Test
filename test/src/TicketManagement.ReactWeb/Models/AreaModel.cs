using System.Collections.Generic;

namespace TicketManagement.ReactWeb.Models
{
    public class AreaModel
    {
        public int Id { get; set; }

        public int LayoutId { get; set; }

        public string Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public string Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<SeatModel> Seats { get; set; } = new ();
    }
}
