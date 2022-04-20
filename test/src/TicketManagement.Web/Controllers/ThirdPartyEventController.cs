using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.Web.Clients;
using TicketManagement.Web.Infrastructure.Filters;
using TicketManagement.Web.Infrastructure.Identity;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Controllers
{
    [Authorize(Roles = Roles.EventManager)]
    public class ThirdPartyEventController : Controller
    {
        private readonly IEventManagerClient _eventClient;
        private readonly IMapper _mapper;
        private readonly IEventService _eventService;
        private readonly ILayoutService _layoutService;
        private readonly IVenueService _venueService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThirdPartyEventController"/> class.
        /// </summary>
        /// <param name="eventClient">Event client.</param>
        /// <param name="mapper">Automapper.</param>
        /// <param name="eventService">Event service.</param>
        /// <param name="layoutService">Layout service.</param>
        /// <param name="venueService">Venue service.</param>
        public ThirdPartyEventController(IEventManagerClient eventClient, IMapper mapper, IEventService eventService,
            ILayoutService layoutService, IVenueService venueService)
        {
            _eventClient = eventClient;
            _mapper = mapper;
            _eventService = eventService;
            _layoutService = layoutService;
            _venueService = venueService;
        }

        [ReactRedirectFilter]
        public async Task<ActionResult> Import(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                var array = memoryStream.ToArray();
                var eventsJson = Encoding.UTF8.GetString(array, 0, array.Length);
                List<ThirdPartyEventModel> events;
                try
                {
                    events = JsonConvert.DeserializeObject<List<ThirdPartyEventModel>>(eventsJson);
                    if (events.Any())
                    {
                        var result = await _eventClient.Import(events);
                        return result ? Ok() : BadRequest("Something wrong during import");
                    }

                    return BadRequest("Can't import from empty file");
                }
                catch
                {
                    return BadRequest("Incorrect or empty file");
                }
            }
        }
    }
}
