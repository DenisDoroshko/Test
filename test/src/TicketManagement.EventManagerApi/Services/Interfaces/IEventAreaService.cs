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
    /// Responsible for proxy event area repository calls.
    /// </summary>
    public interface IEventAreaService
    {
        /// <summary>
        /// Updates event area.
        /// </summary>
        /// <param name="eventAreaModel">Event area model.</param>
        /// <returns>Validation result.</returns>
        Task UpdateEventAreaAsync(EventAreaModel eventAreaModel);

        /// <summary>
        /// Gets area from data storage by specified id.
        /// </summary>
        /// <param name="eventAreaId">Specified id of the event area.</param>
        /// <returns>Event area dto if the operation was successful otherwise null.</returns>
        Task<EventAreaModel> GetEventAreaAsync(int eventAreaId);

        /// <summary>
        /// Gets all event areas from data storage.
        /// </summary>
        /// <returns>Event area list if the operation was successful otherwise empty event area list.</returns>
        List<EventAreaModel> GetEventAreas();

        /// <summary>
        /// Gets specified event areas.
        /// </summary>
        /// <returns>Specified Event area list if the operation was successful otherwise empty event area list.</returns>
        List<EventAreaModel> GetEventAreasBy(Expression<Func<EventArea, bool>> predicate);
    }
}
