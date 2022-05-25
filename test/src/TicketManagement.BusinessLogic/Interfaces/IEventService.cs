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
    /// Responsible for proxy all event repository calls with validation logic.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Creates event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateEventAsync(EventDto eventDto);

        /// <summary>
        /// Updates event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateEventAsync(EventDto eventDto);

        /// <summary>
        /// Deletes event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteEventAsync(EventDto eventDto);

        /// <summary>
        /// Gets event from data storage by specified id.
        /// </summary>
        /// <param name="eventDtoId">Specified id of the event.</param>
        /// <returns>Event dto if the operation was successful otherwise null.</returns>
        Task<EventDto> GetEventAsync(int eventDtoId);

        /// <summary>
        /// Gets all events from data storage.
        /// </summary>
        /// <returns>Event list if the operation was successful otherwise empty event list.</returns>
        List<EventDto> GetEvents();

        /// <summary>
        /// Gets specified events.
        /// </summary>
        /// <returns>Specified Event list if the operation was successful otherwise empty event list.</returns>
        List<EventDto> GetEventsBy(Expression<Func<Event, bool>> predicate);
    }
}
