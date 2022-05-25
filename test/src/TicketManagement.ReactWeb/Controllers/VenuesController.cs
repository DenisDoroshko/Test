using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.ReactWeb.Clients;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VenuesController : Controller
    {
        private readonly IEventManagerClient _eventClient;

        public VenuesController(IEventManagerClient eventClient)
        {
            _eventClient = eventClient;
        }

        [HttpGet]
        public async Task<IEnumerable<VenueModel>> GetEvents()
        {
            return await _eventClient.GetVenues();
        }
    }
}
