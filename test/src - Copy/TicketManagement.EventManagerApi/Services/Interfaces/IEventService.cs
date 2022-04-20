using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FluentValidation.Results;
using TicketManagement.DataAccess.Entities;
using TicketManagement.EventManagerApi.Models;

namespace TicketManagement.EventManagerApi.Services.Interfaces
{
    /// <summary>
    /// Responsible for proxy all event repository calls with validation logic.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Creates event with validation.
        /// </summary>
        /// <param name="eventModel">Event model.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateEventAsync(EventModel eventModel);

        /// <summary>
        /// Updates event with validation.
        /// </summary>
        /// <param name="eventModel">Event model.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateEventAsync(EventModel eventModel);

        /// <summary>
        /// Deletes event with validation.
        /// </summary>
        /// <param name="eventModel">Event model.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteEventAsync(EventModel eventModel);

        /// <summary>
        /// Gets event from data storage by specified id.
        /// </summary>
        /// <param name="eventModelId">Specified id of the event.</param>
        /// <returns>Event model if the operation was successful otherwise null.</returns>
        Task<EventModel> GetEventAsync(int eventModelId);

        /// <summary>
        /// Gets all events from data storage.
        /// </summary>
        /// <returns>Event list if the operation was successful otherwise empty event list.</returns>
        List<EventModel> GetEvents();

        /// <summary>
        /// Gets specified events.
        /// </summary>
        /// <returns>Specified Event list if the operation was successful otherwise empty event list.</returns>
        List<EventModel> GetEventsBy(Expression<Func<Event, bool>> predicate);
    }
}
