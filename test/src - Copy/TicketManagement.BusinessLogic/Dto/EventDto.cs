using System;
using System.Collections.Generic;

namespace TicketManagement.BusinessLogic.Dto
{
    /// <summary>
    /// Repesents event dto.
    /// </summary>
    public class EventDto
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
        /// Image event.
        /// </summary>
        public string Image { get; set; }
    }
}
