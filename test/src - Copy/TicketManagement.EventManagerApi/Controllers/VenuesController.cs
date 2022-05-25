using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Controllers
{
    /// <summary>
    /// Represents api for venues.
    /// </summary>
    [Route("api/[controller]")]
    public class VenuesController : Controller
    {
        private readonly IVenueService _venueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenuesController"/> class.
        /// </summary>
        /// <param name="venueService">Venue service.</param>
        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        [HttpGet]
        public IEnumerable<VenueModel> Get()
        {
            return _venueService.GetVenues();
        }
    }
}
