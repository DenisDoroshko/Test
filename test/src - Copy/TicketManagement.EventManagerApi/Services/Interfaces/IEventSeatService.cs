using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventManagerApi.Models;

namespace TicketManagement.EventManagerApi.Services.Interfaces
{
    /// <summary>
    /// Responsible for proxy event seat repository calls.
    /// </summary>
    public interface IEventSeatService
    {
        /// <summary>
        /// Updates seat with validation.
        /// </summary>
        /// <param name="eventSeat">Event seat.</param>
        /// <returns>Validation result.</returns>
        Task UpdateEventSeatAsync(EventSeatModel eventSeat);

        /// <summary>
        /// Gets event seat from data storage by specified id.
        /// </summary>
        /// <param name="eventSeatId">Specified id of the event seat.</param>
        /// <returns>Event seat dto if the operation was successful otherwise null.</returns>
        Task<EventSeatModel> GetEventSeatAsync(int eventSeatId);

        /// <summary>
        /// Gets all event seats from data storage.
        /// </summary>
        /// <returns>Event seat list if the operation was successful otherwise empty event seat list.</returns>
        List<EventSeatModel> GetEventSeats();

        /// <summary>
        /// Gets specified event seats.
        /// </summary>
        /// <returns>Specified Event seat list if the operation was successful otherwise empty event seat list.</returns>
        List<EventSeatModel> GetEventSeatsBy(Expression<Func<EventSeat, bool>> predicate);
    }
}
