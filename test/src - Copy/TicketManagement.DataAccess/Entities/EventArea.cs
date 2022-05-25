using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    /// <summary>
    /// Reperesents event area.
    /// </summary>
    public class EventArea
    {
        /// <summary>
        /// Event area id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of the area event.
        /// </summary>
        public int EventId { get; set; }

        /// <summary>
        /// Event area description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// X coordinate of the event area location.
        /// </summary>
        public int CoordX { get; set; }

        /// <summary>
        /// Y coordinate of the event area location.
        /// </summary>
        public int CoordY { get; set; }

        /// <summary>
        /// Event area color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Event area price.
        /// </summary>
        public decimal Price { get; set; }
    }
}
