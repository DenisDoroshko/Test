using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.BusinessLogic.Dto
{
    /// <summary>
    /// Represents venue dto.
    /// </summary>
    public class VenueDto
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
