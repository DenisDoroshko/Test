using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Controllers
{
    /// <summary>
    /// Represents api for event management.
    /// </summary>
    [Route("api/[controller]")]
    public class EventsController : BaseServiceController
    {
        private readonly IImageProcesser _imageProcesser;
        private readonly IWebHostEnvironment _environment;
        private readonly IEventService _eventService;
        private readonly IEventAreaService _eventAreaService;
        private readonly IEventSeatService _eventSeatService;
        private readonly IVenueService _venueService;
        private readonly ILayoutService _layoutService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventsController"/> class.
        /// </summary>
        /// <param name="environment">Environment.</param>
        /// <param name="imageProcesser">Image processer.</param>
        /// <param name="eventService">Event service.</param>
        /// <param name="eventAreaService">Event area service.</param>
        /// <param name="eventSeatService">Event seat service.</param>
        /// <param name="venueService">Venue service.</param>
        /// <param name="layoutService">Layout service.</param>
        public EventsController(IImageProcesser imageProcesser,
            IWebHostEnvironment environment, IEventService eventService,
            IEventAreaService eventAreaService, IEventSeatService eventSeatService, IVenueService venueService, ILayoutService layoutService)
        {
            _imageProcesser = imageProcesser;
            _environment = environment;
            _eventService = eventService;
            _eventAreaService = eventAreaService;
            _eventSeatService = eventSeatService;
            _venueService = venueService;
            _layoutService = layoutService;
        }

        /// <summary>
        /// Gets all events.
        /// </summary>
        /// <returns>Events list.</returns>
        [HttpGet]
        public async Task<IEnumerable<EventModel>> Get()
        {
            var events = _eventService.GetEvents();
            foreach (var eventItem in events)
            {
                eventItem.Image = await _imageProcesser.GetBase64Image(eventItem.Image);
            }

            return events;
        }

        /// <summary>
        /// Gets event by id.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Event item.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var eventModel = await _eventService.GetEventAsync(id);

            if (eventModel == null)
            {
                return NotFound();
            }

            return new ObjectResult(eventModel);
        }

        /// <summary>
        /// Adds new event to data storage.
        /// </summary>
        /// <param name="eventModel">Event.</param>
        /// <returns>Result of operation.</returns>
        [Authorize(Roles = Roles.EventManager)]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] EventModel eventModel)
        {
            ValidationResult result;
            if (eventModel == null)
            {
                result = GetValidationResultWithError("Event not found");
                return NotFound(result);
            }

            result = await _eventService.CreateEventAsync(eventModel);
            if (result.IsValid && !string.IsNullOrEmpty(eventModel.Image))
            {
                await AttachImage(eventModel.Id, new ImageModel { Image = eventModel.Image });
            }

            return result.IsValid ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Updates information about event.
        /// </summary>
        /// <param name="eventModel">Event.</param>
        /// <returns>Result of operation.</returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EventModel eventModel)
        {
            ValidationResult result;
            if (eventModel == null || await _eventService.GetEventAsync(eventModel.Id) == null)
            {
                result = GetValidationResultWithError("Event not found");
                return NotFound(result);
            }

            result = await _eventService.UpdateEventAsync(eventModel);
            return result.IsValid ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Deletes event by id.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Result of operation.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ValidationResult result;
            var eventModel = await _eventService.GetEventAsync(id);
            if (eventModel == null)
            {
                result = GetValidationResultWithError("Event not found");
                return NotFound(result);
            }

            result = await _eventService.DeleteEventAsync(eventModel);
            return result.IsValid ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Imports thirdparty events.
        /// </summary>
        /// <param name="events">Events.</param>
        /// <returns>Result of operation.</returns>
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] List<ThirdPartyEventModel> events)
        {
            foreach (var thirdEvent in events)
            {
                var venue = _venueService.GetVenuesBy(v => v.Name == thirdEvent.VenueName).FirstOrDefault();
                var layout = venue != null ? _layoutService.GetLayoutsBy(l => l.Name == thirdEvent.LayoutName && l.VenueId == venue.Id).FirstOrDefault() : null;
                if (layout != null)
                {
                    var eventModel = new EventModel
                    {
                        Name = thirdEvent.Name,
                        Description = thirdEvent.Description,
                        Start = thirdEvent.StartDate,
                        Finish = thirdEvent.EndDate,
                        LayoutId = layout.Id,
                    };
                    var result = await _eventService.CreateEventAsync(eventModel);
                    if (result.IsValid)
                    {
                        await AttachImage(eventModel.Id, new ImageModel { Image = eventModel.Image });
                    }
                }
            }

            return Ok();
        }

        /// <summary>
        /// Attaches image to event.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <param name="model">Image model.</param>
        /// <returns>Result of operation.</returns>
        [HttpPut("{id}/image")]
        public async Task<IActionResult> AttachImage([FromRoute] int id, [FromBody] ImageModel model)
        {
            if (string.IsNullOrEmpty(model.Image))
            {
                return BadRequest();
            }

            var eventItem = await _eventService.GetEventAsync(id);
            if (eventItem != null)
            {
                var imageName = $"{eventItem.Id}.png";
                var imagePath = Path.Combine(_environment.WebRootPath, "eventImages", imageName);
                var savingResult = await _imageProcesser.SaveBase64Image(imagePath, model.Image);
                if (!savingResult)
                {
                    return BadRequest();
                }

                eventItem.Image = $"eventImages/{imageName}";
                var result = await _eventService.UpdateEventAsync(eventItem);
                if (result.IsValid)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Gets event areas by event id.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Event areas.</returns>
        [HttpGet("{id}/areas")]
        public IEnumerable<EventAreaModel> GetEventAreas(int id)
        {
            return id != 0 ? _eventAreaService.GetEventAreasBy(a => a.EventId == id)
                : _eventAreaService.GetEventAreas();
        }

        /// <summary>
        /// Gets event seats by event id.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Event seats list.</returns>
        [HttpGet("{id}/seats")]
        public IEnumerable<EventSeatModel> GetEventSeats(int id)
        {
            return id != 0 ? GetEventSeatsForEvent(id)
                : _eventSeatService.GetEventSeats();
        }

        /// <summary>
        /// Gets event seats by event id.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>Events seats list.</returns>
        private List<EventSeatModel> GetEventSeatsForEvent(int eventId)
        {
            var eventAreaIds = _eventAreaService.GetEventAreasBy(a => a.EventId == eventId).Select(a => a.Id);
            var eventSeats = new List<EventSeatModel>();

            foreach (var areaId in eventAreaIds)
            {
                eventSeats.AddRange(_eventSeatService.GetEventSeatsBy(s => s.EventAreaId == areaId));
            }

            return eventSeats;
        }
    }
}
