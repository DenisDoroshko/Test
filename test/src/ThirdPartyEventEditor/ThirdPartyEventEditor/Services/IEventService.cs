using System.Collections.Generic;
using ThirdPartyEventEditor.Models;
using FluentValidation.Results;

namespace ThirdPartyEventEditor.Services
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
        ValidationResult CreateEvent(ThirdPartyEvent eventEntity);

        /// <summary>
        /// Updates event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        ValidationResult UpdateEvent(ThirdPartyEvent eventEntity);

        /// <summary>
        /// Deletes event with validation.
        /// </summary>
        /// <param name="eventDto">Event dto.</param>
        /// <returns>Validation result.</returns>
        ValidationResult DeleteEvent(ThirdPartyEvent eventEntity);

        /// <summary>
        /// Gets event from data storage by specified id.
        /// </summary>
        /// <param name="eventDtoId">Specified id of the event.</param>
        /// <returns>Event dto if the operation was successful otherwise null.</returns>
        ThirdPartyEvent GetEvent(int eventEntityId);

        /// <summary>
        /// Gets all events from data storage.
        /// </summary>
        /// <returns>Event list if the operation was successful otherwise empty event list.</returns>
        List<ThirdPartyEvent> GetEvents();
    }
}
