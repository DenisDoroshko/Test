using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.Web.Infrastructure.Identity;

namespace TicketManagement.Web.Controllers
{
    /// <summary>
    /// Represents actions for working with seats.
    /// </summary>
    [Authorize(Roles = Roles.VenueManager)]
    public class SeatController : Controller
    {
        private readonly IAreaService _areaService;
        private readonly ISeatService _seatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeatController"/> class.
        /// </summary>
        /// <param name="areaService">Area service.</param>
        /// <param name="seatService">Seat service.</param>
        public SeatController(IAreaService areaService, ISeatService seatService)
        {
            _areaService = areaService;
            _seatService = seatService;
        }

        /// <summary>
        /// Gets Create view.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <param name="number">Seat number.</param>
        /// <param name="row">Seat row.</param>
        /// <returns>Seat id if seat is valid otherwise BadRequest.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(int areaId, int number, int row)
        {
            var area = _areaService.GetAreaAsync(areaId);
            if (area != null && number != 0 && row != 0)
            {
                var seat = new SeatDto
                {
                    AreaId = areaId,
                    Number = number,
                    Row = row,
                };
                await _seatService.CreateSeatAsync(seat);
                return Content(seat.Id.ToString());
            }
            else
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Deletes seat by id.
        /// </summary>
        /// <param name="seatId">Seat id.</param>
        /// <returns>Ok if operation was successful otherwise BadRequest.</returns>
        public async Task<ActionResult> Delete(int seatId)
        {
            var seat = await _seatService.GetSeatAsync(seatId);
            if (seat != null)
            {
                await _seatService.DeleteSeatAsync(seat);
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
