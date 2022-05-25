using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Clients
{
    /// <summary>
    /// Provides operations for interacting with the EventManager api.
    /// </summary>
    public class EventManagerClient : BaseClient, IEventManagerClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventManagerClient"/> class.
        /// </summary>
        /// <param name="httpContextAccessor">Http accessor.</param>
        /// <param name="httpClient">Http client.</param>
        public EventManagerClient(IHttpContextAccessor httpContextAccessor, HttpClient httpClient)
            : base(httpContextAccessor, httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<bool> AttachImage(int id, string image, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(new { image });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PutAsync($"api/events/{id}/image", httpContent, cancellationToken);

            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<EventModel> Get(int id, CancellationToken cancellationToken = default)
        {
            var address = $"api/events/{id}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            var jsonEvent = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<EventModel>(jsonEvent);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventModel>> Get(CancellationToken cancellationToken = default)
        {
            var address = $"api/events";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonEvents = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<List<EventModel>>(jsonEvents);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventAreaModel>> GetEventAreas(int eventId = 0, CancellationToken cancellationToken = default)
        {
            var address = eventId > 0 ? $"api/events/{eventId}/areas" : $"api/areas";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonAreas = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<List<EventAreaModel>>(jsonAreas);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<VenueModel>> GetVenues(CancellationToken cancellationToken = default)
        {
            var result = await _httpClient.GetAsync("api/venues", cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonVenues = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<List<VenueModel>>(jsonVenues);
        }

        /// <inheritdoc />
        public async Task<LayoutModel> GetLayout(int layoutId, CancellationToken cancellationToken = default)
        {
            var address = $"api/layouts/{layoutId}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            var jsonLayout = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<LayoutModel>(jsonLayout);
        }

        /// <inheritdoc />
        public async Task<EventAreaModel> GetEventArea(int eventAreaId, CancellationToken cancellationToken = default)
        {
            var address = $"api/areas/{eventAreaId}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            var jsonSeat = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<EventAreaModel>(jsonSeat);
        }

        /// <inheritdoc />
        public async Task<ValidationResult> Post(EventModel eventModel, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(eventModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PostAsync($"api/events", httpContent, cancellationToken);
            var jsonResult = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<ValidationResult>(jsonResult);
        }

        /// <inheritdoc />
        public async Task<bool> Import(List<ThirdPartyEventModel> events, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(events);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PostAsync($"api/events/import", httpContent, cancellationToken);

            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<ValidationResult> Put(EventModel eventModel, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(eventModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PutAsync($"api/events", httpContent, cancellationToken);
            var jsonResult = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<ValidationResult>(jsonResult);
        }

        /// <inheritdoc />
        public async Task<bool> Put(EventSeatModel eventSeatModel, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(eventSeatModel);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PutAsync($"api/seats", httpContent, cancellationToken);

            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<ValidationResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var address = $"api/events/{id}";
            var result = await _httpClient.DeleteAsync(address, cancellationToken);
            var jsonResult = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<ValidationResult>(jsonResult);
        }

        /// <inheritdoc />
        public async Task<bool> SetAreaPrice(int eventAreaId, decimal price, CancellationToken cancellationToken = default)
        {
            var jsonContent = JsonConvert.SerializeObject(new { price });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, MediaTypeNames.Application.Json);
            var result = await _httpClient.PutAsync($"api/areas/{eventAreaId}/price", httpContent, cancellationToken);

            return result.IsSuccessStatusCode;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EventSeatModel>> GetEventSeats(int eventId = 0, CancellationToken cancellationToken = default)
        {
            var address = eventId > 0 ? $"api/events/{eventId}/seats" : $"api/seats";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            result.EnsureSuccessStatusCode();
            var jsonSeats = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<List<EventSeatModel>>(jsonSeats);
        }

        /// <inheritdoc />
        public async Task<EventSeatModel> GetEventSeat(int eventSeatId, CancellationToken cancellationToken = default)
        {
            var address = $"api/seats/{eventSeatId}";
            var result = await _httpClient.GetAsync(address, cancellationToken);
            var jsonSeat = await result.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<EventSeatModel>(jsonSeat);
        }
    }
}
