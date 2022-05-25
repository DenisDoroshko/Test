using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    /// <summary>
    /// Represents venue.
    /// </summary>
    public class Venue
    {
        /// <summary>
        /// Venue id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Venue name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Venue description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Venue address.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// Venue phone.
        /// </summary>
        public string Phone { get; set; }
    }
}
