using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Web.Models
{
    public class AreaViewModel
    {
        public int Id { get; set; }

        public int LayoutId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public int CoordX { get; set; }

        public int CoordY { get; set; }

        public string Color { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<SeatViewModel> Seats { get; set; }
    }
}
