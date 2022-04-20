using System.Collections.Generic;

namespace TicketManagement.Web.Models
{
    public class CartViewModel
    {
        public string ReturnUrl { get; set; } = "/";

        public List<TicketViewModel> Tickets { get; set; } = new List<TicketViewModel>();
    }
}
