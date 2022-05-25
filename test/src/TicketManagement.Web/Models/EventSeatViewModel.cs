namespace TicketManagement.Web.Models
{
    public enum SeatStates
    {
        Available = 0,
        Booked = 1,
    }

    public class EventSeatViewModel
    {
        public int Id { get; set; }

        public int EventAreaId { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        public SeatStates State { get; set; }

        public string UserId { get; set; }
    }
}
