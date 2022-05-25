using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.BusinessLogic.Dto
{
    /// <summary>
    /// Represents area seat dto.
    /// </summary>
    public class SeatDto
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
