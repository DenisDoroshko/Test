using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.ReactWeb.Clients;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventsController : Controller
    {
        private readonly IStringLocalizer<EventsController> _localizer;
        private readonly IEventManagerClient _eventClient;

        public EventsController(IStringLocalizer<EventsController> localizer, IEventManagerClient eventClient)
        {
            _localizer = localizer;
            _eventClient = eventClient;
        }

        [HttpGet]
        public async Task<IEnumerable<EventModel>> GetEvents()
        {
            var events = await _eventClient.Get();
            var publishedEvents = new List<EventModel>();
            foreach (var eventItem in events)
            {
                var eventAreas = await _eventClient.GetEventAreas(eventItem.Id);
                if (eventAreas.All(a => a.Price > 0))
                {
                    publishedEvents.Add(eventItem);
                }
            }

            return publishedEvents;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var eventModel = await _eventClient.Get(id);

            if (eventModel == null)
            {
                return NotFound();
            }

            eventModel.EventAreas = (await _eventClient.GetEventAreas(id)).ToList();
            foreach (var eventArea in eventModel.EventAreas)
            {
                eventArea.EventSeats = (await _eventClient.GetEventSeats(eventArea.EventId)).ToList();
            }

            return new ObjectResult(eventModel);
        }

        [Authorize(Roles = Roles.EventManager)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EventModel model)
        {
            var response = new Response<string>();
            var result = await _eventClient.Post(model);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    response.Errors.Add(_localizer[error.ErrorMessage]);
                }

                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = Roles.EventManager)]
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] EventModel model)
        {
            var response = new Response<string>();
            var result = await _eventClient.Put(model);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    response.Errors.Add(_localizer[error.ErrorMessage]);
                }

                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = Roles.EventManager)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = new Response<string>();
            var result = await _eventClient.Delete(id);
            if (!result.IsValid)
            {
                foreach (var error in result.Errors)
                {
                    response.Errors.Add(_localizer[error.ErrorMessage]);
                }

                return BadRequest(response);
            }

            return Ok(response);
        }

        [Authorize(Roles = Roles.EventManager)]
        [HttpPost("import")]
        public async Task<IActionResult> Import([FromBody] List<ThirdPartyEventModel> events)
        {
            var result = await _eventClient.Import(events);

            return result ? Ok() : BadRequest();
        }

        [Authorize(Roles = Roles.EventManager)]
        [HttpPut("{id}/image")]
        public async Task<IActionResult> AttachImage([FromRoute] int id, [FromBody] ImageModel model)
        {
            if (string.IsNullOrEmpty(model?.Image))
            {
                return BadRequest();
            }

            var result = await _eventClient.AttachImage(id, model.Image);
            return result ? Ok() : BadRequest();
        }
    }
}
