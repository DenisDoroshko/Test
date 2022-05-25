using System.Web.Mvc;
using ThirdPartyEventEditor.Services;
using ThirdPartyEventEditor.Filters;

namespace ThirdPartyEventEditor.Controllers
{
    /// <summary>
    /// Represents basic actions.
    /// </summary>
    [LogTimeFilter]
    [GlobalExceptionFilter]
    public class HomeController : Controller
    {
        private readonly IEventService _eventService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="eventService">Event service.</param>
        public HomeController(IEventService eventService)
        {
            _eventService = eventService;
        }

        /// <summary>
        /// Gets the Index view.
        /// </summary>
        /// <returns>Index view.</returns>
        public ActionResult Index()
        {
            return View(_eventService.GetEvents());
        }

        /// <summary>
        /// Gets the Error view.
        /// </summary>
        /// <param name="message">Error message.</param>
        /// <returns>Error view.</returns>
        public ActionResult Error(string message)
        {
            ViewData["Error message"] = message;
            return View();
        }
    }
}