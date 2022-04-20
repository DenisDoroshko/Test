using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    /// <summary>
    /// Represents event seat.
    /// </summary>
    public class EventSeat
    {
        /// <summary>
        /// Event seat id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Event seat area id.
        /// </summary>
        public int EventAreaId { get; set; }

        /// <summary>
        /// Event seat row.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Event seat number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Event seat state.
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// User id.
        /// </summary>
        public string UserId { get; set; }
    }
}
