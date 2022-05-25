using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Clients
{
    /// <summary>
    /// Provides operations for interacting with the EventManager api.
    /// </summary>
    public interface IEventManagerClient
    {
        /// <summary>
        /// Gets event by id.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event model.</returns>
        Task<EventModel> Get(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all events.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Events list.</returns>
        Task<IEnumerable<EventModel>> Get(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event areas by event id.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event areas list.</returns>
        Task<IEnumerable<EventAreaModel>> GetEventAreas(int eventId = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets venues.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event areas list.</returns>
        Task<IEnumerable<VenueModel>> GetVenues(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets layout by id.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event areas list.</returns>
        Task<LayoutModel> GetLayout(int layoutId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event area by id.
        /// </summary>
        /// <param name="eventAreaId">Event area id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event area.</returns>
        Task<EventAreaModel> GetEventArea(int eventAreaId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event seats by event id.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event seats list.</returns>
        Task<IEnumerable<EventSeatModel>> GetEventSeats(int eventId = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event seat by id.
        /// </summary>
        /// <param name="eventSeatId">Event seat id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Event seat.</returns>
        Task<EventSeatModel> GetEventSeat(int eventSeatId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates event.
        /// </summary>
        /// <param name="eventModel">Event.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<ValidationResult> Post(EventModel eventModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Imports third party events.
        /// </summary>
        /// <param name="events">Events.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> Import(List<ThirdPartyEventModel> events, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates event.
        /// </summary>
        /// <param name="eventModel">Event.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<ValidationResult> Put(EventModel eventModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates seat.
        /// </summary>
        /// <param name="eventSeatModel">Event seat.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> Put(EventSeatModel eventSeatModel, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes event.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<ValidationResult> Delete(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets price to area.
        /// </summary>
        /// <param name="eventAreaId">Event area id.</param>
        /// <param name="price">Event area price.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> SetAreaPrice(int eventAreaId, decimal price, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attaches image to event.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <param name="image">Image in base64 format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result of operation.</returns>
        Task<bool> AttachImage(int id, string image, CancellationToken cancellationToken = default);
    }
}
