using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Controllers
{
    /// <summary>
    /// Represents api for event areas management.
    /// </summary>
    [Route("api/[controller]")]
    public class AreasController : BaseServiceController
    {
        private readonly IEventAreaService _eventAreaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreasController"/> class.
        /// </summary>
        /// <param name="eventAreaService">Event area service.</param>
        public AreasController(IEventAreaService eventAreaService)
        {
            _eventAreaService = eventAreaService;
        }

        /// <summary>
        /// Gets all event areas.
        /// </summary>
        /// <returns>Event areas.</returns>
        [HttpGet]
        public IEnumerable<EventAreaModel> Get()
        {
            return _eventAreaService.GetEventAreas();
        }

        /// <summary>
        /// Gets event area by id.
        /// </summary>
        /// <param name="id">Event area id.</param>
        /// <returns>Event area.</returns>
        [HttpGet("{id}")]
        public async Task<EventAreaModel> GetEventArea(int id)
        {
            return await _eventAreaService.GetEventAreaAsync(id);
        }

        /// <summary>
        /// Sets price to event area.
        /// </summary>
        /// <param name="id">Event area id.</param>
        /// <param name="model">Price model.</param>
        /// <returns>Result of operation.</returns>
        [Authorize(Roles = Roles.EventManager)]
        [HttpPut("{id}/price")]
        public async Task<IActionResult> SetAreaPrice([FromRoute] int id, [FromBody] SetPriceModel model)
        {
            if (model == null || model.Price <= 0)
            {
                return BadRequest();
            }

            var eventArea = _eventAreaService.GetEventAreasBy(a => a.Id == id).FirstOrDefault();
            if (eventArea == null)
            {
                return BadRequest();
            }

            eventArea.Price = model.Price;
            await _eventAreaService.UpdateEventAreaAsync(eventArea);
            return Ok();
        }
    }
}
