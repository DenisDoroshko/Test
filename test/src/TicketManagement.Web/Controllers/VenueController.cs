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
    /// Represents action for working with venues.
    /// </summary>
    [Authorize(Roles = Roles.VenueManager)]
    public class VenueController : Controller
    {
        private readonly IStringLocalizer<VenueController> _localizer;
        private readonly IMapper _mapper;
        private readonly IVenueService _venueService;
        private readonly ILayoutService _layoutService;
        private readonly IAreaService _areaService;
        private readonly ISeatService _seatService;

        /// <summary>
        /// Initializes a new instance of the <see cref="VenueController"/> class.
        /// </summary>
        /// <param name="localizer">Localizer.</param>
        /// <param name="mapper">Automapper.</param>
        /// <param name="venueService">Venue service.</param>
        /// <param name="layoutService">Layout service.</param>
        /// <param name="areaService">Area service.</param>
        /// <param name="seatService">Seat service.</param>
        public VenueController(IStringLocalizer<VenueController> localizer, IMapper mapper, IVenueService venueService,
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
        /// Gets the Venues view.
        /// </summary>
        /// <returns>Venues view.</returns>
        [AllowAnonymous]
        public ActionResult Venues()
        {
            var venuesList = _venueService.GetVenues();
            var venueModels = _mapper.Map<List<VenueDto>, List<VenueViewModel>>(venuesList);
            return View(venueModels);
        }

        /// <summary>
        /// Gets the ManageVenues view.
        /// </summary>
        /// <returns>Manage venues view.</returns>
        public ActionResult ManageVenues()
        {
            var venues = _venueService.GetVenues();
            var venueModels = _mapper.Map<List<VenueDto>, List<VenueViewModel>>(venues);
            return View(venueModels);
        }

        /// <summary>
        /// Gets view for creating a venue.
        /// </summary>
        /// <returns>Create view.</returns>
        public ActionResult Create()
        {
            return View(new VenueViewModel());
        }

        /// <summary>
        /// Creats venue view model.
        /// </summary>
        /// <param name="venueModel">Venue view model.</param>
        /// <returns>Redirect ot ManageVenues view if venue is valid
        /// otherwise Create view with validation errors.</returns>
        [HttpPost]
        public async Task<ActionResult> Create(VenueViewModel venueModel)
        {
            if (ModelState.IsValid)
            {
                var venueDto = _mapper.Map<VenueViewModel, VenueDto>(venueModel);
                var validationResult = await _venueService.CreateVenueAsync(venueDto);
                if (validationResult.IsValid)
                {
                    return RedirectToAction("ManageVenues", "Venue");
                }
                else
                {
                    foreach (var error in validationResult.Errors)
                    {
                        ModelState.AddModelError("", _localizer[error.ErrorMessage]);
                    }
                }
            }

            return View(venueModel);
        }

        /// <summary>
        /// Gets the Edit view.
        /// </summary>
        /// <param name="venueId">Venue id.</param>
        /// <returns>Edit view if venue id is correct otherwise redirect Error view.</returns>
        public async Task<ActionResult> Edit(int venueId)
        {
            var venueDto = await _venueService.GetVenueAsync(venueId);
            if (venueDto != null)
            {
                var venueModel = MapVenue(venueDto);
                return View(venueModel);
            }
            else
            {
                return RedirectToAction("Error", "Home",
                    new { message = _localizer["Venue not found"] });
            }
        }

        /// <summary>
        /// Edits venue.
        /// </summary>
        /// <param name="venueModel">Venue view model.</param>
        /// <returns>Edit view with result messages.</returns>
        [HttpPost]
        public async Task<ActionResult> Edit(VenueViewModel venueModel)
        {
            if (ModelState.IsValid)
            {
                var venueDto = _mapper.Map<VenueViewModel, VenueDto>(venueModel);
                var validationResult = await _venueService.UpdateVenueAsync(venueDto);
                venueModel = MapVenue(venueDto);
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

            return View(venueModel);
        }

        /// <summary>
        /// Deletes venus by id.
        /// </summary>
        /// <param name="venueId">Venue id.</param>
        /// <returns>Partial view with result messages.</returns>
        public async Task<ActionResult> Delete(int venueId)
        {
            var messages = new List<string>();
            var venue = await _venueService.GetVenueAsync(venueId);
            var validationResult = await _venueService.DeleteVenueAsync(venue);
            if (validationResult != null && validationResult.IsValid)
            {
                messages.Add(_localizer["Successfull operation"]);
            }
            else
            {
                foreach (var error in validationResult?.Errors)
                {
                    messages.Add(_localizer[error.ErrorMessage]);
                }
            }

            ViewData["Header"] = _localizer["Venue deleting"];

            return PartialView("_ModalViewPartial", messages);
        }

        /// <summary>
        /// Gets the Details view.
        /// </summary>
        /// <param name="venueId">Venue id.</param>
        /// <returns>Details view if venue id is correct otherwise redirect to Error view.</returns>
        [AllowAnonymous]
        public async Task<ActionResult> Details(int venueId)
        {
            var venue = await _venueService.GetVenueAsync(venueId);
            var venueModel = _mapper.Map<VenueDto, VenueViewModel>(venue);
            if (venueModel != null)
            {
                return View(venueModel);
            }
            else
            {
                return RedirectToAction("Error", "Home",
                    new { message = _localizer["Venue not found"] });
            }
        }

        /// <summary>
        /// Converts venue dto to venue view model.
        /// </summary>
        /// <param name="venueDto">Venue dto.</param>
        /// <returns>Venue view model.</returns>
        private VenueViewModel MapVenue(VenueDto venueDto)
        {
            var venueModel = _mapper.Map<VenueDto, VenueViewModel>(venueDto);

            var dtoLayouts = _layoutService.GetLayoutsBy(l =>l.VenueId == venueModel.Id).ToList();
            venueModel.Layouts = _mapper.Map<List<LayoutDto>, List<LayoutViewModel>>(dtoLayouts);
            venueModel.Layouts.ForEach(l => l.Areas =
                _mapper.Map<List<AreaDto>, List<AreaViewModel>>(_areaService
                .GetAreasBy(a => a.LayoutId == l.Id).ToList()));

            venueModel.Layouts.ForEach(l => l.Areas.ForEach(a => a.Seats =
                _mapper.Map<List<SeatDto>, List<SeatViewModel>>(_seatService
                .GetSeatsBy(s => s.AreaId == a.Id).ToList())));
            return venueModel;
        }
    }
}
