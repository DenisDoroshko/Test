using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.EventManagerApi.Models;
using TicketManagement.EventManagerApi.Services.Interfaces;

namespace TicketManagement.EventManagerApi.Controllers
{
    /// <summary>
    /// Represents api for layouts.
    /// </summary>
    [Route("api/[controller]")]
    public class LayoutsController : Controller
    {
        private readonly ILayoutService _layoutService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutsController"/> class.
        /// </summary>
        /// <param name="layoutService">Layout service.</param>
        public LayoutsController(ILayoutService layoutService)
        {
            _layoutService = layoutService;
        }

        [HttpGet("{id}")]
        public async Task<LayoutModel> Get(int id)
        {
            return await _layoutService.GetLayoutAsync(id);
        }
    }
}
