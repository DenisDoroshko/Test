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
    /// Represents actions for working with layouts.
    /// </summary>
    [Authorize(Roles = Roles.VenueManager)]
    public class LayoutController : Controller
    {
        private readonly IStringLocalizer<LayoutController> _localizer;
        private readonly IMapper _mapper;
        private readonly IVenueService _venueService;
        private readonly ILayoutService _layoutService;
        private readonly IAreaService _areaService;
        private readonly ISeatService _seatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutController"/> class.
        /// </summary>
        /// <param name="localizer">Localizer.</param>
        /// <param name="mapper">Automapper.</param>
        /// <param name="venueService">Venue service.</param>
        /// <param name="layoutService">Layout service.</param>
        /// <param name="areaService">Area service.</param>
        /// <param name="seatService">Seat service.</param>
        public LayoutController(IStringLocalizer<LayoutController> localizer, IMapper mapper, IVenueService venueService,
        ILayoutService layoutService, IAreaService areaService, ISeatService seatService)
        {
            _localizer = localizer;
            _mapper = mapper;
            _venueService = venueService;
            _layoutService = layoutService;
            _areaService = areaService;
            _seatService = seatService;
        }

        /// <summary>
        /// Gets view for creating a layout.
        /// </summary>
        /// <param name="venueId">Venue id.</param>
        /// <returns>Create layout view.</returns>
        public async Task<ActionResult> Create(int venueId)
        {
            var venue = await _venueService.GetVenueAsync(venueId);
            if (venue != null)
            {
                return View(new LayoutViewModel { VenueId = venueId });
            }
            else
            {
                return RedirectToAction("Error", "Home",
                    new { message = _localizer["Venue for layout not found"] });
            }
        }

        /// <summary>
        /// Creates layout.
        /// </summary>
        /// <param name="layoutModel">Layout view model.</param>
        /// <returns>Redirect to edit layout view if layout is valid otherwise create
        /// view with validation errors.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(LayoutViewModel layoutModel)
        {
            if (ModelState.IsValid)
            {
                var layoutDto = _mapper.Map<LayoutViewModel, LayoutDto>(layoutModel);
                var validationResult = await _layoutService.CreateLayoutAsync(layoutDto);
                if (validationResult.IsValid)
                {
                    return RedirectToAction("Edit", new { layoutId = layoutDto.Id });
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                    }
                }
            }

            return View(layoutModel);
        }

        /// <summary>
        /// Gets the Edit view.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>Edit view if layout id is correct otherwise Error view.</returns>
        public async Task<ActionResult> Edit(int layoutId)
        {
            var layoutDto = await _layoutService.GetLayoutAsync(layoutId);
            if (layoutDto != null)
            {
                var layoutModel = MapLayout(layoutDto);
                return View(layoutModel);
            }
            else
            {
                return RedirectToAction("Error", "Home",
                    new { message = _localizer["Layout for editing not found"] });
            }
        }

        /// <summary>
        /// Edits layout.
        /// </summary>
        /// <param name="layoutModel">Layout view model.</param>
        /// <returns>Edit view with result messages.</returns>
        [HttpPost]
        public async Task<ActionResult> Edit(LayoutViewModel layoutModel)
        {
            if (ModelState.IsValid)
            {
                var layoutDto = _mapper.Map<LayoutViewModel, LayoutDto>(layoutModel);
                var validationResult = await _layoutService.UpdateLayoutAsync(layoutDto);
                layoutModel = MapLayout(layoutDto);
                if (validationResult.IsValid)
                {
                    ViewData["SuccessMessage"] = _localizer["Information has been successfully edited"];
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                    }
                }
            }

            return View(layoutModel);
        }

        /// <summary>
        /// Deletes layout by id.
        /// </summary>
        /// <param name="layoutId">Layout id.</param>
        /// <returns>Partial view with result messages.</returns>
        public async Task<ActionResult> Delete(int layoutId)
        {
            var messages = new List<string>();
            var layout = await _layoutService.GetLayoutAsync(layoutId);
            var validationResult = await _layoutService.DeleteLayoutAsync(layout);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    messages.Add(error.ErrorMessage);
                }
            }
            else
            {
                messages.Add(_localizer["Successfull operation"]);
            }

            ViewData["Header"] = _localizer["Layout deleting"];

            return PartialView("_ModalViewPartial", messages);
        }

        /// <summary>
        /// Converts layout dto to layout view model.
        /// </summary>
        /// <param name="layoutDto">Layout dto.</param>
        /// <returns>Layout view model.</returns>
        private LayoutViewModel MapLayout(LayoutDto layoutDto)
        {
            var layoutModel = _mapper.Map<LayoutDto, LayoutViewModel>(layoutDto);
            var dtoAreas = _areaService.GetAreasBy(a => a.LayoutId == layoutDto.Id).ToList();
            layoutModel.Areas = _mapper.Map<List<AreaDto>, List<AreaViewModel>>(dtoAreas);
            layoutModel.Areas.ForEach(area => area.Seats =
                _mapper.Map<List<SeatDto>, List<SeatViewModel>>(_seatService
                .GetSeatsBy(s => s.AreaId == area.Id).ToList()));
            return layoutModel;
        }
    }
}
