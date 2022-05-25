using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using TicketManagement.BusinessLogic.Dto;
using TicketManagement.BusinessLogic.Interfaces;
using TicketManagement.Web.Infrastructure.Identity;
using TicketManagement.Web.Models;

namespace TicketManagement.Web.Controllers
{
    /// <summary>
    /// Represents actions for working with areas.
    /// </summary>
    public class AreaController : Controller
    {
        private readonly IStringLocalizer<AreaController> _localizer;
        private readonly IMapper _mapper;
        private readonly ILayoutService _layoutService;
        private readonly IAreaService _areaService;
        private readonly ISeatService _seatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AreaController"/> class.
        /// </summary>
        /// <param name="localizer">Localizer.</param>
        /// <param name="mapper">Automapper.</param>
        /// <param name="layoutService">Layout service.</param>
        /// <param name="areaService">Area service.</param>
        /// <param name="seatService">Seat service.</param>
        public AreaController(IStringLocalizer<AreaController> localizer, IMapper mapper, ILayoutService layoutService, IAreaService areaService, ISeatService seatService)
        {
            _localizer = localizer;
            _mapper = mapper;
            _layoutService = layoutService;
            _areaService = areaService;
            _seatService = seatService;
        }

        /// <summary>
        /// Gets the view for creating a new area.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>Create view if layout id is correct otherwise Error view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        public async Task<ActionResult> Create(int layoutId)
        {
            var layout = await _layoutService.GetLayoutAsync(layoutId);
            if (layout != null)
            {
                return View(new AreaViewModel { LayoutId = layoutId });
            }
            else
            {
                return RedirectToAction("Error", "Home", new { message = _localizer["Layout not found"] });
            }
        }

        /// <summary>
        /// Creates a new area.
        /// </summary>
        /// <param name="areaModel">Area view model.</param>
        /// <returns>Edit view if operations was successful otherwise
        /// Create view with validation errors.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpPost]
        public async Task<ActionResult> Create(AreaViewModel areaModel)
        {
            if (ModelState.IsValid)
            {
                var areaDto = _mapper.Map<AreaViewModel, AreaDto>(areaModel);
                var validationResult = await _areaService.CreateAreaAsync(areaDto);
                if (validationResult.IsValid)
                {
                    await CreateSeats(areaDto);
                    return RedirectToAction("Edit", new { areaId = areaDto.Id });
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                    }
                }
            }

            return View(areaModel);
        }

        /// <summary>
        /// Gets the view for editing an area.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <returns>Edit view if area id is correct otherwise Error view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        public async Task<ActionResult> Edit(int areaId)
        {
            var areaDto = await _areaService.GetAreaAsync(areaId);
            if (areaDto != null)
            {
                var areaModel = MapArea(areaDto);
                return View(areaModel);
            }
            else
            {
                return RedirectToAction("Error", "Home", new { message = _localizer["Area not found"] });
            }
        }

        /// <summary>
        /// Edits area.
        /// </summary>
        /// <param name="areaModel">Area view model.</param>
        /// <returns>Edit view with result messages.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpPost]
        public async Task<ActionResult> Edit(AreaViewModel areaModel)
        {
            if (ModelState.IsValid)
            {
                var areaDto = _mapper.Map<AreaViewModel, AreaDto>(areaModel);
                var validationResult = await _areaService.UpdateAreaAsync(areaDto);
                areaModel.Seats = await DeleteSeats(areaModel.Id, areaModel.Width, areaModel.Height);

                if (validationResult.IsValid)
                {
                    ViewData["SuccessMessage"] = _localizer["Information successfully edited"];
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                    }
                }
            }

            return View(areaModel);
        }

        /// <summary>
        /// Places area on layout.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <param name="coordX">Coordinate x.</param>
        /// <param name="coordY">Coordinate y.</param>
        /// <returns>Ok if operation was successful otherwise BadRequest.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        [HttpPost]
        public async Task<ActionResult> Place(int areaId, int coordX, int coordY)
        {
            var areaDto = await _areaService.GetAreaAsync(areaId);
            if (areaDto != null)
            {
                areaDto.CoordX = coordX;
                areaDto.CoordY = coordY;
                var validationResult = await _areaService.UpdateAreaAsync(areaDto);
                if (validationResult.IsValid)
                {
                    return Ok();
                }
            }

            return BadRequest(_localizer["Placement error"].Value);
        }

        /// <summary>
        /// Deletes area by id.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <returns>Redirect to edit layout view.</returns>
        [Authorize(Roles = Roles.VenueManager)]
        public async Task<ActionResult> Delete(int areaId)
        {
            var area = await _areaService.GetAreaAsync(areaId);
            await _areaService.DeleteAreaAsync(area);
            return RedirectToAction("Edit", "Layout", new { layoutId = area.LayoutId });
        }

        /// <summary>
        /// Deletes area seats that row and number more then area size.
        /// </summary>
        /// <param name="areaId">Area id.</param>
        /// <param name="width">Area width.</param>
        /// <param name="height">Area height.</param>
        /// <returns>Area seats list.</returns>
        private async Task<List<SeatViewModel>> DeleteSeats(int areaId, int width, int height)
        {
            var areaSeats = _seatService.GetSeatsBy(s => s.AreaId == areaId).ToList();
            for (var i = 0; i < areaSeats.Count; i++)
            {
                if (areaSeats[i].Number > width || areaSeats[i].Row > height)
                {
                    await _seatService.DeleteSeatAsync(areaSeats[i]);
                }
            }

            var dtoSeats = _seatService.GetSeatsBy(s => s.AreaId == areaId).ToList();
            return _mapper.Map<List<SeatDto>, List<SeatViewModel>>(dtoSeats);
        }

        /// <summary>
        /// Creates area seats if area has no yet.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Task.</returns>
        private async Task CreateSeats(AreaDto areaDto)
        {
            for (var i = 1; i <= areaDto.Width; i++)
            {
                for (var j = 1; j <= areaDto.Height; j++)
                {
                    await _seatService.CreateSeatAsync(new SeatDto { AreaId = areaDto.Id, Number = i, Row = j });
                }
            }
        }

        /// <summary>
        /// Converts area dto to area view model.
        /// </summary>
        /// <param name="areaDto">Area dto.</param>
        /// <returns>Area view model.</returns>
        private AreaViewModel MapArea(AreaDto areaDto)
        {
            var areaModel = _mapper.Map<AreaDto, AreaViewModel>(areaDto);
            var dtoSeats = _seatService.GetSeatsBy(s => s.AreaId == areaModel.Id).ToList();
            areaModel.Seats = _mapper.Map<List<SeatDto>, List<SeatViewModel>>(dtoSeats);
            return areaModel;
        }
    }
}
