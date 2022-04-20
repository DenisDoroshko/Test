using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.ReactWeb.Clients;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LayoutsController : Controller
    {
        private readonly IEventManagerClient _eventClient;

        public LayoutsController(IEventManagerClient eventClient)
        {
            _eventClient = eventClient;
        }

        [HttpGet("{id}")]
        public async Task<LayoutModel> GetLayout(int id)
        {
            return await _eventClient.GetLayout(id);
        }
    }
}
