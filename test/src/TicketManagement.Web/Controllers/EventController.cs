using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.Web.Clients;
using TicketManagement.Web.Infrastructure.Filters;
using TicketManagement.Web.Infrastructure.Helpers;
using TicketManagement.Web.Infrastructure.Identity;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Controllers
{
    /// <summary>
    /// Represent action for working with events.
    /// </summary>
    [Authorize(Roles = Roles.EventManager)]
    public class EventController : Controller
    {
        private readonly IEventManagerClient _eventClient;
        private readonly IStringLocalizer<EventController> _localizer;
        private readonly TimeZoneHelper _timeZoneHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="eventClient">Event manager client.</param>
        /// <param name="localizer">Localizer.</param>
        /// <param name="timeZoneHelper">Time zone helper.</param>
        public EventController(IEventManagerClient eventClient,
            IStringLocalizer<EventController> localizer, TimeZoneHelper timeZoneHelper)
        {
            _eventClient = eventClient;
            _localizer = localizer;
            _timeZoneHelper = timeZoneHelper;
        }

        /// <summary>
        /// Gets the view for creating an event.
        /// </summary>
        /// <returns>Create event view.</returns>
        [ReactRedirectFilter]
        public ActionResult Create()
        {
            return View(new EventViewModel());
        }

        /// <summary>
        /// Creates an event.
        /// </summary>
        /// <param name="eventModel">Event view model.</param>
        /// <param name="image">Event image.</param>
        /// <returns>Redirect to manage events view if event is valid otherwise Create
        /// view with validation errors.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(EventViewModel eventModel, IFormFile image)
        {
            if (ModelState.IsValid)
            {
                eventModel.Start = await _timeZoneHelper.ConvertToUtcAsync(eventModel.Start);
                eventModel.Finish = await _timeZoneHelper.ConvertToUtcAsync(eventModel.Finish);
                if (image != null && image.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        eventModel.Image = Convert.ToBase64String(memoryStream.ToArray());
                    }
                }

                var validationResult = await _eventClient.Post(eventModel);
                if (validationResult.IsValid)
                {
                    return RedirectToAction(nameof(ManageEvents));
                }

                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                }
            }

            return View(eventModel);
        }

        /// <summary>
        /// Gets the ManageEvents viewn with all events.
        /// </summary>
        /// <returns>ManageEvents view with all events.</returns>
        [ReactRedirectFilter]
        public async Task<ActionResult> ManageEvents()
        {
            var events = await _eventClient.Get();
            return View(events);
        }

        /// <summary>
        /// Gets Events view with available events.
        /// </summary>
        /// <returns>Events view with available events.</returns>
        [ReactRedirectFilter]
        [AllowAnonymous]
        public async Task<ActionResult> Events()
        {
            var events = await _eventClient.Get();
            var publishedEvents = new List<EventViewModel>();
            foreach (var eventItem in events)
            {
                var eventAreas = await _eventClient.GetEventAreas(eventItem.Id);
                if (eventAreas.All(a => a.Price > 0))
                {
                    publishedEvents.Add(eventItem);
                }
            }

            return View(publishedEvents);
        }

        /// <summary>
        /// Gets the Deteils view.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>Details view if event id is correct otherwise redirect to Error view.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> Details(int eventId)
        {
            var eventModel = await _eventClient.Get(eventId);

            if (eventModel != null)
            {
                await MapEvent(eventModel);
                return View(eventModel);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { message = _localizer["Event not found"] });
            }
        }

        /// <summary>
        /// Gets edit event view.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>Edit view if event id is correct otherwise Error view.</returns>
        [ReactRedirectFilter]
        public async Task<ActionResult> Edit(int eventId)
        {
            var eventModel = await _eventClient.Get(eventId);
            if (eventModel != null)
            {
                eventModel.EventAreas = (await _eventClient.GetEventAreas(eventId)).ToList();
                foreach (var eventArea in eventModel.EventAreas)
                {
                    eventArea.EventSeats = (await _eventClient.GetEventSeats(eventArea.EventId)).ToList();
                }

                return View(eventModel);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { message = _localizer["Event not found"] });
            }
        }

        /// <summary>
        /// Edits event.
        /// </summary>
        /// <param name="eventModel">Event view model.</param>
        /// <returns>Edit view with result messages.</returns>
        [HttpPost]
        public async Task<ActionResult> Edit(EventViewModel eventModel)
        {
            if (ModelState.IsValid)
            {
                eventModel.Start = await _timeZoneHelper.ConvertToUtcAsync(eventModel.Start);
                eventModel.Finish = await _timeZoneHelper.ConvertToUtcAsync(eventModel.Finish);
                var validationResult = await _eventClient.Put(eventModel);
                if (validationResult.IsValid)
                {
                    ViewData["SuccessMessage"] = _localizer["Information successfully edited"];
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                    }
                }
            }

            await MapEvent(eventModel);

            return View(eventModel);
        }

        /// <summary>
        /// Sets price to event area.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <param name="eventAreaId">Event area id.</param>
        /// <param name="price">Event area price.</param>
        /// <returns>Redirect to edit event view.</returns>
        [HttpPost]
        public async Task<ActionResult> SetPrice(int eventId, int eventAreaId, decimal price)
        {
            await _eventClient.SetAreaPrice(eventAreaId, price);

            return RedirectToAction("Edit", "Event", new { eventId = eventId });
        }

        /// <summary>
        /// Attahes image to event.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <param name="image">Event image.</param>
        /// <returns>Partial view with operation result.</returns>
        [HttpPost]
        public async Task<ActionResult> AttachImage(int eventId, IFormFile image)
        {
            if (image != null && image.Length > 0)
            {
                var eventItem = await _eventClient.Get(eventId);
                if (eventItem != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await image.CopyToAsync(memoryStream);
                        var stringImage = Convert.ToBase64String(memoryStream.ToArray());
                        await _eventClient.AttachImage(eventId, stringImage);
                    }
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { message = _localizer["Event not found"] });
                }
            }

            return RedirectToAction("Edit", new { eventId = eventId });
        }

        /// <summary>
        /// Deletes event by id.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <returns>Partial view with operation result.</returns>
        public async Task<ActionResult> Delete(int eventId)
        {
            var messages = new List<string>();
            var validationResult = await _eventClient.Delete(eventId);
            if (validationResult != null && validationResult.IsValid)
            {
                messages.Add(_localizer["Successfull operation"]);
            }
            else
            {
                foreach (var error in validationResult?.Errors)
                {
                    messages.Add(_localizer[error.ErrorMessage]);
                }
            }

            ViewData["Header"] = _localizer["Event deleting"];

            return PartialView("_ModalViewPartial", messages);
        }

        /// <summary>
        /// Gets data related with event.
        /// </summary>
        /// <param name="eventModel">Event view model.</param>
        private async Task MapEvent(EventViewModel eventModel)
        {
            eventModel.EventAreas = (await _eventClient.GetEventAreas(eventModel.Id)).ToList();
            var eventSeats = (await _eventClient.GetEventSeats(eventModel.Id)).ToList();
            foreach (var eventArea in eventModel.EventAreas)
            {
                eventArea.EventSeats = eventSeats.Where(s => s.EventAreaId == eventArea.Id).ToList();
            }
        }
    }
}
