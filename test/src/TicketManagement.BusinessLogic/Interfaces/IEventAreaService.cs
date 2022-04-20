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
    /// Responsible for proxy all event area repository calls with validation logic.
    /// </summary>
    public interface IEventAreaService
    {
        /// <summary>
        /// Creates event area with validation.
        /// </summary>
        /// <param name="eventAreaDto">Event area dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> CreateEventAreaAsync(EventAreaDto eventAreaDto);

        /// <summary>
        /// Updates area with validation.
        /// </summary>
        /// <param name="eventAreaDto">Event area dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> UpdateEventAreaAsync(EventAreaDto eventAreaDto);

        /// <summary>
        /// Deletes area with validation.
        /// </summary>
        /// <param name="eventAreaDto">Event area dto.</param>
        /// <returns>Validation result.</returns>
        Task<ValidationResult> DeleteEventAreaAsync(EventAreaDto eventAreaDto);

        /// <summary>
        /// Gets area from data storage by specified id.
        /// </summary>
        /// <param name="eventAreaId">Specified id of the event area.</param>
        /// <returns>Event area dto if the operation was successful otherwise null.</returns>
        Task<EventAreaDto> GetEventAreaAsync(int eventAreaId);

        /// <summary>
        /// Gets all event areas from data storage.
        /// </summary>
        /// <returns>Event area list if the operation was successful otherwise empty event area list.</returns>
        List<EventAreaDto> GetEventAreas();

        /// <summary>
        /// Gets specified event areas.
        /// </summary>
        /// <returns>Specified Event area list if the operation was successful otherwise empty event area list.</returns>
        List<EventAreaDto> GetEventAreasBy(Expression<Func<EventArea, bool>> predicate);
    }
}
