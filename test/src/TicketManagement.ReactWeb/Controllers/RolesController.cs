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
    public class RolesController : Controller
    {
        private readonly IStringLocalizer<LoginsController> _localizer;
        private readonly IRolesClient _rolesClient;

        public RolesController(IStringLocalizer<LoginsController> localizer, IUserClient userClient, IRolesClient rolesClient)
        {
            _localizer = localizer;
            _rolesClient = rolesClient;
        }

        [HttpPost("{role}/{id}")]
        [Authorize(Roles.VenueManager)]
        public async Task<ActionResult> AddRole(string id, string role)
        {
            var result = await _rolesClient.AddToTole(id, role);
            return result ? Ok() : BadRequest();
        }

        [HttpDelete("{role}/{id}")]
        [Authorize(Roles.VenueManager)]
        public async Task<ActionResult> DeleteRole(string id, string role)
        {
            var result = await _rolesClient.DeleteFromTole(id, role);
            return result ? Ok() : BadRequest();
        }
    }
}
