using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ThirdPartyEventEditor.Services;
using ThirdPartyEventEditor.Models;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using Newtonsoft.Json;
using System.Web.Hosting;
using ThirdPartyEventEditor.Filters;
using ThirdPartyEventEditor.App_Start;

namespace ThirdPartyEventEditor.Controllers
{
    /// <summary>
    /// Represent actions for working with events.
    /// </summary>
    [LogTimeFilter]
    [GlobalExceptionFilter]
    public class EventController : Controller
    {
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventController"/> class.
        /// </summary>
        /// <param name="eventService">Event service</param>
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Gets the view for creating an event.
        /// </summary>
        /// <returns>Create event view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public ActionResult Create()
        {
            return View(new ThirdPartyEvent());
        }

        /// <summary>
        /// Creates an event
        /// </summary>
        /// <param name="eventModel">Event model</param>
        /// <param name="image">Event image</param>
        /// <returns>Redirect to manage events view if event is valid otherwise Create
        /// view with validation errors.</returns>
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult> Create(ThirdPartyEvent eventModel, HttpPostedFileWrapper image)
        {
            if (!ModelState.IsValid)
            {
                return View(eventModel);
            }

            eventModel.StartDate = eventModel.StartDate.ToUniversalTime();
            eventModel.EndDate = eventModel.EndDate.ToUniversalTime();
            eventModel.PosterImage = image == null ? null : await UploadImage(image);
            var result = _eventService.CreateEvent(eventModel);
            if (result.IsValid)
            {
                return RedirectToAction(nameof(ManageEvents));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.ErrorMessage);
            }
            
            return View(eventModel);
        }

        /// <summary>
        /// Gets the Deteils view.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Details view if event id is correct otherwise redirect to Error view.</returns>
        public ActionResult Details(int id = 0)
        {
            var eventItem = _eventService.GetEvent(id);
            if (eventItem != null)
            {
                return View(eventItem);
            }

            return RedirectToAction("Error", "Home", new { message = "Event not found" });
        }

        /// <summary>
        /// Deletes event by id.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Redirect to ManageEvents view if operation was successful otherwise Error view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public ActionResult Delete(int id = 0)
        {
            var eventModel = _eventService.GetEvent(id);
            if(eventModel != null)
            {
                var result = _eventService.DeleteEvent(eventModel);
                if (result.IsValid)
                {
                    return RedirectToAction(nameof(ManageEvents));
                }

                return RedirectToAction("Error", "Home");
            }

            return RedirectToAction("Error", "Home", new { message = "Event not found" });
        }

        /// <summary>
        /// Gets edit event view.
        /// </summary>
        /// <param name="id">Event id.</param>
        /// <returns>Edit view if event id is correct otherwise Error view.</returns>
        [Authorize(Roles = Roles.Admin)]
        public ActionResult Edit(int id = 0)
        {
            var eventModel = _eventService.GetEvent(id);
            if(eventModel != null)
            {
                return View(eventModel);
            }

            return RedirectToAction("Error", "Home", new { message = "Event not found" });
        }

        /// <summary>
        /// Edits event.
        /// </summary>
        /// <param name="eventModel">Event view model.</param>
        /// <returns>Edit view with result messages.</returns>
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public ActionResult Edit(ThirdPartyEvent eventModel)
        {
            if (!ModelState.IsValid)
            {
                return View(eventModel);
            }

            eventModel.StartDate = eventModel.StartDate.ToUniversalTime();
            eventModel.EndDate = eventModel.EndDate.ToUniversalTime();
            var result = _eventService.UpdateEvent(eventModel);
            if (result.IsValid)
            {
                ViewData["SuccessMessage"] = "Information successfully edited";
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.ErrorMessage);
                }
            }

            return View(eventModel);

        }

        /// <summary>
        /// Exports events as json file.
        /// </summary>
        /// <returns>Json file.</returns>
        [Authorize(Roles = Roles.Admin)]
        public FileResult Export()
        {
            var events = _eventService.GetEvents();
            var eventsDto = new List<ThirdPartyEventDto>();
            events.ForEach(e => eventsDto.Add(new ThirdPartyEventDto
            {
                Name = e.Name,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                Description = e.Description,
                PosterImage = e.PosterImage.Replace("data:image/png;base64,", ""),
                VenueName = e.VenueName,
                LayoutName = e.LayoutName,
            }));
            var eventsJson = JsonConvert.SerializeObject(eventsDto, Formatting.Indented);
            var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, ConfigurationManager.AppSettings["ExportPath"]);
            System.IO.File.WriteAllText(path, eventsJson);
            string file_type = "application/json";
            string file_name = "Events.json";
            return File(path, file_type, file_name);
        }

        /// <summary>
        /// Attahes image to event.
        /// </summary>
        /// <param name="eventId">Event id.</param>
        /// <param name="image">Event image.</param>
        /// <returns>Partial view with operation result.</returns>
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult> AttachImage(int eventId, HttpPostedFileWrapper image)
        {
            if (image != null && image.ContentLength > 0)
            {
                var eventModel = _eventService.GetEvent(eventId);
                if (eventModel != null)
                {
                    eventModel.PosterImage = await UploadImage(image);
                    _eventService.UpdateEvent(eventModel);
                }
                else
                {
                    return RedirectToAction("Error", "Home", new { message = "Event not found" });
                }
            }

            return RedirectToAction("Edit", new { id = eventId });
        }

        /// <summary>
        /// Gets the ManageEvents viewn with all events.
        /// </summary>
        /// <returns>ManageEvents view with all events.</returns>
        [Authorize(Roles = Roles.Admin)]
        public ActionResult ManageEvents()
        {
            return View(_eventService.GetEvents());
        }

        /// <summary>
        /// Uploads image and converts to base64 type.
        /// </summary>
        /// <param name="image">Image from form.</param>
        /// <returns>Base64 image.</returns>
        private async Task<string> UploadImage(HttpPostedFileWrapper image)
        {
            using (var memoryStream = new MemoryStream())
            {
                await image.InputStream.CopyToAsync(memoryStream);
                return "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
            }
        }
    }
}