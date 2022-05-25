using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Controllers
{
    /// <summary>
    /// Represents api for event seats management.
    /// </summary>
    [Route("api/[controller]")]
    public class SeatsController : BaseServiceController
    {
        private readonly IEventSeatService _eventSeatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatsController"/> class.
        /// </summary>
        /// <param name="eventSeatService">Event seat service.</param>
        public SeatsController(IEventSeatService eventSeatService)
        {
            _eventSeatService = eventSeatService;
        }

        /// <summary>
        /// Gets all event seats.
        /// </summary>
        /// <returns>Event seats list.</returns>
        [HttpGet]
        public IEnumerable<EventSeatModel> Get()
        {
            return _eventSeatService.GetEventSeats();
        }

        /// <summary>
        /// Gets event seat by id.
        /// </summary>
        /// <param name="id">Event seat id.</param>
        /// <returns>Event seat.</returns>
        [HttpGet("{id}")]
        public async Task<EventSeatModel> Get(int id)
        {
            return await _eventSeatService.GetEventSeatAsync(id);
        }

        /// <summary>
        /// Updates event seat.
        /// </summary>
        /// <param name="eventSeatModel">Event seat.</param>
        /// <returns>Result of operation.</returns>
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] EventSeatModel eventSeatModel)
        {
            if (eventSeatModel == null || await _eventSeatService.GetEventSeatAsync(eventSeatModel.Id) == null)
            {
                return NotFound();
            }

            await _eventSeatService.UpdateEventSeatAsync(eventSeatModel);

            return Ok();
        }
    }
}
