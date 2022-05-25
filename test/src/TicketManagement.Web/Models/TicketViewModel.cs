using System;

namespace TicketManagement.Web.Models
{
    public class TicketViewModel
    {
        public string EventName { get; set; }

        public DateTime EventStart { get; set; }

        public DateTime PurchaseDate { get; set; }

        public decimal Price { get; set; }

        public string UserId { get; set; }

        public int EventSeatId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }
    }
}
