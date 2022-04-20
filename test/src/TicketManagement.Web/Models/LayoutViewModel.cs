using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Web.Models
{
    public class LayoutViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public int VenueId { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public List<AreaViewModel> Areas { get; set; }
    }
}
