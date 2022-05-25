using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    /// <summary>
    /// Represents area seat.
    /// </summary>
    public class Seat
    {
        /// <summary>
        /// Seat id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Seat area id.
        /// </summary>
        public int AreaId { get; set; }

        /// <summary>
        /// Seat row.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Seat number.
        /// </summary>
        public int Number { get; set; }
    }
}
