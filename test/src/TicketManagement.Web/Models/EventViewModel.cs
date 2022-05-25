using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TicketManagement.Web.Models
{
    public class EventViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Start is required")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "Finish is required")]
        public DateTime Finish { get; set; }

        public int LayoutId { get; set; }

        public string Address { get; set; }

        public string Image { get; set; } = "/eventImages/default.jpg";

        public int Width { get; set; }

        public int Height { get; set; }

        public List<EventAreaViewModel> EventAreas { get; set; }
    }
}
