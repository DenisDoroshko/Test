using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.ReactWeb.Clients;
using TicketManagement.ReactWeb.Models;

namespace TicketManagement.ReactWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AreasController : Controller
    {
        private readonly IEventManagerClient _eventClient;

        public AreasController(IEventManagerClient eventClient)
        {
            _eventClient = eventClient;
        }

        [Authorize(Roles = Roles.EventManager)]
        [HttpPut("{id}/price")]
        public async Task<IActionResult> SetAreaPrice([FromRoute] int id, [FromBody] SetPriceModel model)
        {
            if (model == null || model.Price <= 0)
            {
                return BadRequest();
            }

            var result = await _eventClient.SetAreaPrice(id, model.Price);

            return result ? Ok() : BadRequest();
        }
    }
}
