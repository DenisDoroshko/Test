using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.Web.Clients;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Controllers
{
    /// <summary>
    /// Represents basic actions.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IEventManagerClient _eventClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="eventClient">Event manager client.</param>
        public HomeController(IEventManagerClient eventClient)
        {
            _eventClient = eventClient;
        }

        /// <summary>
        /// Gets the Index view.
        /// </summary>
        /// <returns>Index view.</returns>
        public async Task<ActionResult> Index()
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

            return View(publishedEvents.OrderBy(e => e.Start).Take(9));
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

        /// <summary>
        /// Changes UI language.
        /// </summary>
        /// <param name="lang">Language.</param>
        /// <param name="returnUrl">Url to return after changing.</param>
        /// <returns>Redirect to returnUrl.</returns>
        [HttpPost]
        public ActionResult SetLanguage(string lang, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return LocalRedirect(returnUrl);
        }
    }
}
