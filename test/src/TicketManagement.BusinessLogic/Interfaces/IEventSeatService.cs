using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.DataAccess.Entities;

namespace TicketManagement.BusinessLogic.Interfaces
{
    /// <summary>
    /// Responsible for proxy all event seat repository calls with validation logic.
    /// </summary>
    public interface IEventSeatService
    {
        /// <summary>
        /// Creates event seat with validation.
        /// </summary>
        /// <param name="eventSeatDto">Event seat dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateEventSeatAsync(EventSeatDto eventSeatDto);

        /// <summary>
        /// Updates seat with validation.
        /// </summary>
        /// <param name="eventSeatDto">Event seat dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateEventSeatAsync(EventSeatDto eventSeatDto);

        /// <summary>
        /// Deletes event seat with validation.
        /// </summary>
        /// <param name="eventSeatDto">Event seat dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteEventSeatAsync(EventSeatDto eventSeatDto);

        /// <summary>
        /// Gets event seat from data storage by specified id.
        /// </summary>
        /// <param name="eventSeatId">Specified id of the event seat.</param>
        /// <returns>Event seat dto if the operation was successful otherwise null.</returns>
        Task<EventSeatDto> GetEventSeatAsync(int eventSeatId);

        /// <summary>
        /// Gets all event seats from data storage.
        /// </summary>
        /// <returns>Event seat list if the operation was successful otherwise empty event seat list.</returns>
        List<EventSeatDto> GetEventSeats();

        /// <summary>
        /// Gets specified event seats.
        /// </summary>
        /// <returns>Specified Event seat list if the operation was successful otherwise empty event seat list.</returns>
        List<EventSeatDto> GetEventSeatsBy(Expression<Func<EventSeat, bool>> predicate);
    }
}
