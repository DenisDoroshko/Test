using System;
using System.Collections.Generic;
using System.Text;

namespace TicketManagement.DataAccess.Entities
{
    /// <summary>
    /// Repesents event.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Event id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Event name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Event description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Event start date and time.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Event finish date and time.
        /// </summary>
        public DateTime Finish { get; set; }

        /// <summary>
        /// Id of the event layout.
        /// </summary>
        public int LayoutId { get; set; }

        /// <summary>
        /// Image of the event.
        /// </summary>
        public string Image { get; set; }
    }
}
