using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Web.Models
{
    public class VenueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone is required")]
        [RegularExpression(@"^\+\d{1,3}\(\d{1,3}\)\d{7}$", ErrorMessage = "Phone must be:+xxx(xxx)xxxxxxx")]
        public string Phone { get; set; }

        public List<LayoutViewModel> Layouts { get; set; }
    }
}
