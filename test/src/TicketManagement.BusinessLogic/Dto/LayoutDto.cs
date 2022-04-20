using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.BusinessLogic.Dto
{
    /// <summary>
    /// Represents venue layout dto.
    /// </summary>
    public class LayoutDto
    {
        /// <summary>
        /// Layout id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Layout name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Layout venue id.
        /// </summary>
        public int VenueId { get; set; }

        /// <summary>
        /// Layout description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Layout width in seats number.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Layout height in seats number.
        /// </summary>
        public int Height { get; set; }
    }
}
