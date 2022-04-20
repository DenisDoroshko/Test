using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    /// <summary>
    /// Represents area of the layout.
    /// </summary>
    public class Area
    {
        /// <summary>
        /// Area id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id of the area layout.
        /// </summary>
        public int LayoutId { get; set; }

        /// <summary>
        /// Description of the area.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// X coordinate of the area location.
        /// </summary>
        public int CoordX { get; set; }

        /// <summary>
        /// Y coordinate of the area location.
        /// </summary>
        public int CoordY { get; set; }

        /// <summary>
        /// Area color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// Area width in seats.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Area height in seats.
        /// </summary>
        public int Height { get; set; }
    }
}
